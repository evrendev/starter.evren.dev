<script setup lang="ts">
import { useTheme } from "vuetify";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  pages: { id: string; completed?: boolean }[];
  activePageId?: string | null;
}>();

const emit = defineEmits<{
  (e: "close"): void;
}>();

const { t } = useI18n();
const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
// Same red-in-both-themes pattern used throughout the player (dark's primary
// is gray, so the mark color comes from secondary there instead)
const markColor = computed(() =>
  isDark.value ? vuetifyTheme.current.value.colors.secondary : vuetifyTheme.current.value.colors.primary,
);
const futureColor = computed(() => vuetifyTheme.current.value.colors["grey-300"]);
const closeIconColor = computed(() => vuetifyTheme.current.value.colors["on-surface"]);
const closeHoverBg = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "14");

type SegmentState = "completed" | "active" | "future";

const segmentState = (page: { id: string; completed?: boolean }): SegmentState => {
  if (page.id === props.activePageId) return "active";
  return page.completed ? "completed" : "future";
};
</script>

<template>
  <div class="progress-segments-bar">
    <div class="segments-row" role="progressbar">
      <div
        v-for="page in pages"
        :key="page.id"
        class="segment"
        :class="`segment--${segmentState(page)}`"
      />
    </div>
    <button
      class="segments-close-btn"
      :aria-label="t('shared.close')"
      type="button"
      @click="emit('close')"
    >
      <v-icon icon="$close" size="20" />
    </button>
  </div>
</template>

<style scoped>
.progress-segments-bar {
  flex-shrink: 0;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 8px 8px 16px;
}

.segments-row {
  flex: 1;
  display: flex;
  gap: 4px;
}

.segment {
  flex: 1;
  height: 4px;
  border-radius: 2px;
  background-color: v-bind(futureColor);
}

.segment--completed {
  background-color: v-bind(markColor);
}

.segment--active {
  background-color: v-bind(markColor);
  opacity: 0.5;
}

.segments-close-btn {
  flex-shrink: 0;
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  border-radius: 50%;
  background: transparent;
  color: v-bind(closeIconColor);
  cursor: pointer;
  transition: background-color 0.2s;
}

.segments-close-btn:hover {
  background-color: v-bind(closeHoverBg);
}
</style>
