<script setup lang="ts">
import { useTheme } from "vuetify";
import { QuestionDto } from "@/types/responses/page";

const props = defineProps<{
  content: string;
  questions?: QuestionDto[];
}>();

interface QuizOption {
  label: string;
  correct: boolean;
}

interface QuizQuestion {
  prompt: string;
  options: QuizOption[];
}

// Legacy fallback parser — pages created before the structural Quiz model
// (see Task N0/N1/N2) still carry plain HTML with the option marked
// "(richtig)" treated as the correct answer. Used only when props.questions
// is empty/undefined (see `parsed` below).
const parsedFromContent = computed(() => {
  const doc = new DOMParser().parseFromString(props.content, "text/html");
  const title = doc.querySelector("h1, h2, h3")?.textContent?.trim() ?? "";
  const questions: QuizQuestion[] = [];

  doc.querySelectorAll("ol, ul").forEach((list) => {
    let promptEl = list.previousElementSibling;
    while (promptEl && promptEl.tagName !== "P") {
      promptEl = promptEl.previousElementSibling;
    }

    const options = [...list.querySelectorAll("li")].map((li) => {
      const raw = li.textContent?.trim() ?? "";
      return {
        label: raw.replace(/\s*\(richtig\)\s*$/i, ""),
        correct: /richtig/i.test(raw),
      };
    });

    if (options.length > 0) {
      questions.push({
        prompt: promptEl?.textContent?.trim() ?? "",
        options,
      });
    }
  });

  return { title, questions };
});

// Structural Quiz data takes priority when present — see props.questions.
// Backend doesn't sort nested collections (Task N1's report), so options are
// sorted by Order here rather than trusted as-is.
const parsed = computed(() => {
  if (!props.questions?.length) return parsedFromContent.value;

  return {
    title: "",
    questions: [...props.questions]
      .sort((a, b) => a.order - b.order)
      .map((q) => ({
        prompt: q.prompt,
        options: [...q.options]
          .sort((a, b) => a.order - b.order)
          .map((o) => ({ label: o.label, correct: o.isCorrect })),
      })),
  };
});

// selections[questionIndex] = chosen option index
const selections = ref<Record<number, number>>({});

watch(
  // Content alone isn't a reliable page identity for structural Quiz pages —
  // Content is often left as an empty placeholder when the admin fills in
  // Questions instead, so two different quiz pages could share the same
  // Content and fail to reset selections on navigation.
  () => [props.content, props.questions],
  () => {
    selections.value = {};
  },
);

const select = (questionIndex: number, optionIndex: number) => {
  selections.value = { ...selections.value, [questionIndex]: optionIndex };
};

const optionState = (questionIndex: number, optionIndex: number, option: QuizOption) => {
  if (selections.value[questionIndex] !== optionIndex) return "neutral";
  return option.correct ? "correct" : "wrong";
};

const vuetifyTheme = useTheme();
const themeColors = computed(() => vuetifyTheme.current.value.colors);
// Red in dark theme is the secondary token (primary is gray there)
const correctChipColor = computed(() =>
  vuetifyTheme.current.value.dark ? "secondary" : "primary",
);
const accentSoft = computed(() => themeColors.value.accent + "33");
const errorSoft = computed(() => themeColors.value.error + "26");
const primaryColor = computed(() => themeColors.value.primary);
const errorColor = computed(() => themeColors.value.error);
const neutralBorder = computed(() => themeColors.value["grey-300"]);
</script>

<template>
  <div class="quiz-content">
    <h2 v-if="parsed.title" class="quiz-title">{{ parsed.title }}</h2>

    <v-card
      v-for="(question, qi) in parsed.questions"
      :key="qi"
      rounded="lg"
      variant="outlined"
      class="quiz-question mb-4"
    >
      <v-card-text>
        <p class="quiz-prompt">{{ question.prompt }}</p>

        <div
          v-for="(option, oi) in question.options"
          :key="oi"
          class="quiz-option"
          :class="`quiz-option--${optionState(qi, oi, option)}`"
          role="radio"
          :aria-checked="selections[qi] === oi"
          tabindex="0"
          @click="select(qi, oi)"
          @keydown.enter.prevent="select(qi, oi)"
          @keydown.space.prevent="select(qi, oi)"
        >
          <span class="quiz-radio" />
          <span class="quiz-label">{{ option.label }}</span>
          <v-chip
            v-if="selections[qi] === oi"
            :color="option.correct ? correctChipColor : 'error'"
            variant="flat"
            size="small"
            :prepend-icon="option.correct ? 'bx-bxs-check-circle' : 'bx-x-circle'"
            class="quiz-chip"
          >
            {{ option.correct ? "Richtig ✓" : "Falsch ✗" }}
          </v-chip>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<style scoped>
.quiz-title {
  margin-bottom: 20px;
}

.quiz-prompt {
  font-weight: 600;
  margin-bottom: 16px;
}

.quiz-option {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  margin-bottom: 8px;
  border: 2px solid v-bind(neutralBorder);
  border-radius: 8px;
  cursor: pointer;
  transition:
    background-color 0.15s,
    border-color 0.15s;
}

.quiz-option:hover {
  border-color: v-bind(primaryColor);
}

.quiz-radio {
  flex-shrink: 0;
  width: 18px;
  height: 18px;
  border: 2px solid v-bind(neutralBorder);
  border-radius: 50%;
  transition:
    border-color 0.15s,
    background-color 0.15s;
}

.quiz-label {
  flex: 1;
}

.quiz-chip {
  flex-shrink: 0;
}

/* Doubled class: state colors must also win over the :hover border rule */
.quiz-option.quiz-option--correct {
  background-color: v-bind(accentSoft);
  border-color: v-bind(primaryColor);
}

.quiz-option--correct .quiz-radio {
  border-color: v-bind(primaryColor);
  background-color: v-bind(primaryColor);
  box-shadow: inset 0 0 0 3px v-bind(accentSoft);
}

.quiz-option.quiz-option--wrong {
  background-color: v-bind(errorSoft);
  border-color: v-bind(errorColor);
}

.quiz-option--wrong .quiz-radio {
  border-color: v-bind(errorColor);
  background-color: v-bind(errorColor);
  box-shadow: inset 0 0 0 3px v-bind(errorSoft);
}
</style>
