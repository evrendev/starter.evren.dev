<script setup lang="ts">
import { VFileUpload } from "vuetify/labs/VFileUpload";
import { Notify } from "@/stores/notification";
import { usePageStore } from "@/stores/page";
import { useChapterStore } from "@/stores/chapter";
import { PageContentType } from "@/models/page";
import {
  CreatePageRequest,
  UpdatePageRequest,
  QuestionRequest,
} from "@/types/requests/page";
import { useConvertImportedContent } from "@/composables/useConvertImportedContent";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const pageStore = usePageStore();
const chapterStore = useChapterStore();
const { pageDetails, loading } = storeToRefs(pageStore);

const pageId = computed(() => route.params.id as string);
const isEdit = computed(() => !!pageId.value);
const chapterId = computed(
  () =>
    (route.params.chapterId as string) || pageDetails.value?.chapterId || "",
);

// Backend still requires non-empty Content (used as the slide caption in the
// player, see LessonPlayer.vue) — Image/Video's caption is optional and
// Embed has no content field at all in this form, so an empty value falls
// back to this at submit time instead
const CONTENT_PLACEHOLDER = "&nbsp;";

interface QuestionFormState {
  prompt: string;
  options: { label: string; isCorrect: boolean }[];
}

// Form state
const form = reactive({
  title: "",
  content: "",
  contentType: PageContentType.Text,
  order: 0,
  mediaUrl: "",
  mediaFile: undefined as File | File[] | undefined,
  isImported: false,
  // Quiz content type: structural questions (see Task N0/N1/N2). Array position
  // doubles as Order on submit — no separate reordering UI, add/remove only.
  questions: [] as QuestionFormState[],
});

const contentTypeOptions = computed(() =>
  Object.values(PageContentType).map((value) => ({
    value,
    title: t(`admin.pages.fields.contentType.options.${value.toLowerCase()}`),
  })),
);

// Text/Quiz share the legacy generic Content editor (Quill, or the isImported
// iframe+raw-HTML view from Task L) — Image/Video/Embed get dedicated,
// type-specific fields instead
const usesGenericContentEditor = computed(
  () =>
    form.contentType === PageContentType.Text ||
    form.contentType === PageContentType.Quiz,
);

const selectedMediaFile = computed<File | undefined>(() =>
  Array.isArray(form.mediaFile) ? form.mediaFile[0] : form.mediaFile,
);

// Persisted MediaUrl is a relative storage path ("Files/Images/Page/x.jpg"), same as
// Course.image — needs the backend base URL prefix to resolve as an <img>/<video> src
// (see views/admin/courses/DataTable.vue's baseUrl usage for the established pattern)
const backendBaseUrl: string = import.meta.env.VITE_APP_BACKEND_BASE_URL;

const mediaPreviewUrl = computed(() => {
  if (selectedMediaFile.value)
    return URL.createObjectURL(selectedMediaFile.value);
  if (!form.mediaUrl) return "";
  return `${backendBaseUrl}/${form.mediaUrl}`;
});

// Skips the very first reset the watcher below would otherwise trigger when
// onMounted assigns the real contentType loaded from the Page — that's not a
// user-driven type change, so nothing should be cleared. Only meaningful in
// edit mode; in create mode there's nothing loaded to protect.
let skipNextContentTypeReset = isEdit.value;

watch(
  () => form.contentType,
  () => {
    if (skipNextContentTypeReset) {
      skipNextContentTypeReset = false;
      return;
    }

    // Switching type discards whatever the old type's fields held — a
    // low-risk action (nothing has been saved yet), so no confirmation
    form.mediaFile = undefined;
    form.mediaUrl = "";
    form.content =
      form.contentType === PageContentType.Embed ? CONTENT_PLACEHOLDER : "";
  },
);

// Tracks whether the admin actually touched the Questions block — mirrors the
// backend's undefined/[]/non-empty semantics (see UpdatePageRequest.questions):
// undefined means "leave existing questions untouched", so we must only send a
// (possibly empty) array when the admin really edited it, never on every save.
const questionsDirty = ref(false);
let skipNextQuestionsDirty = isEdit.value;

watch(
  () => form.questions,
  () => {
    if (skipNextQuestionsDirty) {
      skipNextQuestionsDirty = false;
      return;
    }
    questionsDirty.value = true;
  },
  { deep: true },
);

const addQuestion = () => {
  form.questions.push({ prompt: "", options: [{ label: "", isCorrect: true }] });
};

const removeQuestion = (questionIndex: number) => {
  form.questions.splice(questionIndex, 1);
};

const addOption = (questionIndex: number) => {
  const options = form.questions[questionIndex].options;
  options.push({ label: "", isCorrect: options.length === 0 });
};

const removeOption = (questionIndex: number, optionIndex: number) => {
  form.questions[questionIndex].options.splice(optionIndex, 1);
};

const correctOptionIndex = (questionIndex: number) =>
  form.questions[questionIndex].options.findIndex((o) => o.isCorrect);

const setCorrectOption = (questionIndex: number, optionIndex: number) => {
  form.questions[questionIndex].options.forEach((option, i) => {
    option.isCorrect = i === optionIndex;
  });
};

// Client-side guard so obviously-broken Quiz data never reaches the backend —
// every question needs at least one option and exactly one marked correct.
const questionsValidationError = computed(() => {
  if (form.contentType !== PageContentType.Quiz) return null;

  for (const question of form.questions) {
    if (question.options.length === 0) {
      return t("admin.pages.quiz.validation.optionRequired");
    }
    if (question.options.filter((o) => o.isCorrect).length !== 1) {
      return t("admin.pages.quiz.validation.exactlyOneCorrect");
    }
  }

  return null;
});

const { convertToEditable } = useConvertImportedContent();
const showConvertConfirm = ref(false);

// True once form.isImported reflects the real Page (or immediately in create
// mode, where there's nothing to fetch). Content is only rendered once this is
// true — form.isImported defaults to false, and briefly rendering the
// QuillyEditor branch before the fetch resolves would mount then unmount
// Quill, which injects its toolbar as a raw DOM sibling outside Vue's own
// vnode tree; Vue has no record of it and leaves it orphaned in the DOM when
// the branch switches back to the iframe view.
const contentReady = ref(false);

const handleConvertConfirmed = () => {
  form.content = convertToEditable(form.content);
  // Local-only: nothing is persisted until Save is pressed. Flipping this
  // switches the template's v-if from the iframe+textarea view to the real
  // QuillyEditor so the admin can immediately see/edit the converted result.
  form.isImported = false;
};

onMounted(async () => {
  if (isEdit.value && pageId.value) {
    const result = await pageStore.getPageById(pageId.value);
    if (result.succeeded && pageDetails.value) {
      form.title = pageDetails.value.title;
      // The submit-time placeholder for an empty caption/content (see
      // CONTENT_PLACEHOLDER) round-trips through the backend verbatim — treat it
      // as empty again so the admin doesn't see a literal "&nbsp;" on reload
      form.content =
        pageDetails.value.content === CONTENT_PLACEHOLDER
          ? ""
          : pageDetails.value.content || "";
      form.contentType =
        (pageDetails.value.contentType as PageContentType) ||
        PageContentType.Text;
      form.order = pageDetails.value.order || 0;
      form.mediaUrl = pageDetails.value.mediaUrl || "";
      form.isImported = pageDetails.value.isImported || false;
      // Don't trust the backend's physical row order for either level (see
      // Task N1's report on Mapster's nested-collection projection) — sort by
      // Order explicitly on both questions and their options.
      form.questions = [...(pageDetails.value.questions ?? [])]
        .sort((a, b) => a.order - b.order)
        .map((q) => ({
          prompt: q.prompt,
          options: [...q.options]
            .sort((a, b) => a.order - b.order)
            .map((o) => ({ label: o.label, isCorrect: o.isCorrect })),
        }));
    }
  } else if (route.params.chapterId) {
    await chapterStore.getById(route.params.chapterId as string);
  }
  contentReady.value = true;
});

const pageTitle = computed(() => {
  if (isEdit.value) {
    return t("admin.pages.edit.title");
  }
  return t("admin.pages.create.title");
});

// chapterId isn't known yet on the initial render of the edit route (it only
// comes from the Page once getPageById resolves) — an empty params.id would
// make vue-router throw "Missing required param", so fall back to the
// chapter list until it's available
const backTo = computed(() =>
  chapterId.value
    ? { name: "chapter-view", params: { id: chapterId.value } }
    : { name: "chapter-list" },
);

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { name: "dashboard" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.chapters"),
    to: { path: "/admin/chapters" },
  },
  {
    title: chapterStore.chapter?.title || "",
    to: backTo.value,
  },
  {
    title: isEdit.value ? t("shared.edit") : t("shared.create"),
    disabled: true,
  },
]);

const handleSubmit = async () => {
  try {
    if (questionsValidationError.value) {
      Notify.error(questionsValidationError.value);
      return;
    }

    let response;
    // Content is optional in the Image/Video/Embed UI (caption, or hidden
    // entirely for Embed) but the backend still requires a non-empty string —
    // it doubles as the player's slide caption for every content type
    const content = usesGenericContentEditor.value
      ? form.content
      : form.content?.trim() || CONTENT_PLACEHOLDER;

    // undefined = leave existing Questions untouched (backend semantics — see
    // UpdatePageRequest.questions); only send a list when the admin actually
    // touched the block, empty array included (that's a deliberate "delete
    // all"). Array position doubles as Order — no separate reordering UI.
    const questions: QuestionRequest[] | undefined =
      form.contentType === PageContentType.Quiz && questionsDirty.value
        ? form.questions.map((question, qi) => ({
            prompt: question.prompt,
            order: qi,
            options: question.options.map((option, oi) => ({
              label: option.label,
              isCorrect: option.isCorrect,
              order: oi,
            })),
          }))
        : undefined;

    if (isEdit.value && pageId.value) {
      const payload: UpdatePageRequest = {
        id: pageId.value,
        chapterId: chapterId.value,
        title: form.title,
        content,
        contentType: form.contentType,
        order: form.order,
        mediaUrl: selectedMediaFile.value
          ? undefined
          : form.mediaUrl || undefined,
        mediaFile: selectedMediaFile.value,
        isImported: form.isImported,
        questions,
      };
      response = await pageStore.updatePage(pageId.value, payload);
    } else {
      const payload: CreatePageRequest = {
        chapterId: chapterId.value,
        title: form.title,
        content,
        contentType: form.contentType,
        order: form.order,
        mediaUrl: selectedMediaFile.value
          ? undefined
          : form.mediaUrl || undefined,
        mediaFile: selectedMediaFile.value,
        questions,
      };
      response = await pageStore.createPage(payload);
    }

    if (response.succeeded) {
      const msg = isEdit.value
        ? t("admin.pages.notifications.updated")
        : t("admin.pages.notifications.created");
      Notify.success(msg);
      await router.push(backTo.value);
    } else {
      Notify.error(t("admin.pages.notifications.saveFailed"));
    }
  } catch (error) {
    Notify.error(t("shared.unexpectedError"));
  }
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />

  <v-card elevation="6" class="mt-4" :disabled="loading">
    <v-card-title>
      <toolbar
        color="secondary"
        :title="pageTitle"
        :button="{
          icon: 'bx-chevron-left',
          text: t('shared.back'),
          to: backTo,
        }"
      />
    </v-card-title>

    <v-card-text>
      <v-form @submit.prevent="handleSubmit">
        <v-row>
          <v-col cols="12" md="4">
            <v-text-field
              v-model="form.title"
              :label="t('admin.pages.fields.title.title')"
              :placeholder="t('admin.pages.fields.title.placeholder')"
              outlined
              required
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-select
              v-model="form.contentType"
              :items="contentTypeOptions"
              item-title="title"
              item-value="value"
              :label="t('admin.pages.fields.contentType.title')"
              outlined
            />
          </v-col>

          <v-col cols="12" md="4">
            <v-text-field
              v-model.number="form.order"
              :label="t('admin.pages.fields.order.title')"
              type="number"
              outlined
            />
          </v-col>
        </v-row>

        <!-- Quiz keeps the legacy generic MediaUrl field (unchanged, no
             structured Quiz data model exists yet) -->
        <v-row v-if="form.contentType === PageContentType.Quiz" class="mb-4">
          <v-col cols="12" md="6">
            <v-text-field
              v-model="form.mediaUrl"
              :label="t('admin.pages.fields.mediaUrl.title')"
              :placeholder="t('admin.pages.fields.mediaUrl.placeholder')"
              outlined
            />
          </v-col>
        </v-row>

        <v-row v-if="form.contentType === PageContentType.Quiz" class="mb-4">
          <v-col cols="12">
            <label class="text-subtitle-2 mb-2 d-block">
              {{ t("admin.pages.quiz.title") }}
            </label>

            <v-alert
              v-if="form.questions.length === 0"
              type="info"
              variant="tonal"
              density="compact"
              class="mb-4"
            >
              {{ t("admin.pages.quiz.noQuestions") }}
            </v-alert>

            <v-card
              v-for="(question, qi) in form.questions"
              :key="qi"
              variant="outlined"
              class="mb-4"
            >
              <v-card-text>
                <div class="d-flex gap-2 align-start mb-2">
                  <v-text-field
                    v-model="question.prompt"
                    :label="`${t('admin.pages.quiz.questionPrompt.title')} ${qi + 1}`"
                    :placeholder="t('admin.pages.quiz.questionPrompt.placeholder')"
                    outlined
                    hide-details
                  />
                  <v-btn
                    icon="bx-trash"
                    variant="text"
                    color="error"
                    :aria-label="t('admin.pages.quiz.removeQuestion')"
                    @click="removeQuestion(qi)"
                  />
                </div>

                <v-radio-group
                  :model-value="correctOptionIndex(qi)"
                  hide-details
                  class="mb-2"
                  @update:model-value="(idx) => setCorrectOption(qi, idx as number)"
                >
                  <div
                    v-for="(option, oi) in question.options"
                    :key="oi"
                    class="d-flex gap-2 align-center mb-2"
                  >
                    <v-radio
                      :value="oi"
                      :aria-label="t('admin.pages.quiz.correctAnswer')"
                    />
                    <v-text-field
                      v-model="option.label"
                      :label="`${t('admin.pages.quiz.optionLabel.title')} ${oi + 1}`"
                      :placeholder="t('admin.pages.quiz.optionLabel.placeholder')"
                      outlined
                      hide-details
                      density="compact"
                    />
                    <v-btn
                      icon="bx-x"
                      variant="text"
                      color="error"
                      size="small"
                      :aria-label="t('admin.pages.quiz.removeOption')"
                      @click="removeOption(qi, oi)"
                    />
                  </div>
                </v-radio-group>

                <v-btn
                  variant="tonal"
                  size="small"
                  prepend-icon="bx-plus"
                  @click="addOption(qi)"
                >
                  {{ t("admin.pages.quiz.addOption") }}
                </v-btn>
              </v-card-text>
            </v-card>

            <v-btn
              color="primary"
              variant="tonal"
              prepend-icon="bx-plus"
              @click="addQuestion"
            >
              {{ t("admin.pages.quiz.addQuestion") }}
            </v-btn>
          </v-col>
        </v-row>

        <v-row
          v-if="
            form.contentType === PageContentType.Image ||
            form.contentType === PageContentType.Video
          "
          class="mb-4"
          align="start"
        >
          <v-col cols="12" md="6">
            <label
              class="text-subtitle-2 mb-2 d-block"
              v-text="
                form.contentType === PageContentType.Image
                  ? t('admin.pages.fields.imageFile.title')
                  : t('admin.pages.fields.videoFile.title')
              "
            />
            <v-file-upload
              v-model="form.mediaFile"
              :accept="
                form.contentType === PageContentType.Image
                  ? 'image/*'
                  : 'video/*'
              "
              icon="bx-upload"
              clearable
              scrim="primary"
              density="comfortable"
              :show-size="true"
              :multiple="false"
            />
          </v-col>

          <v-col cols="12" md="6">
            <label class="text-subtitle-2 mb-2 d-block">
              {{ t("admin.pages.fields.caption.title") }}
            </label>
            <v-text-field
              v-model="form.content"
              :placeholder="t('admin.pages.fields.caption.placeholder')"
              outlined
            />

            <template v-if="mediaPreviewUrl">
              <label class="text-subtitle-2 mt-4 mb-2 d-block">
                {{ t("admin.pages.fields.preview.title") }}
              </label>
              <img
                v-if="form.contentType === PageContentType.Image"
                :src="mediaPreviewUrl"
                :alt="form.content"
                class="media-preview-thumbnail"
              />
              <video
                v-else
                :src="mediaPreviewUrl"
                controls
                class="media-preview-thumbnail"
              />
            </template>
          </v-col>
        </v-row>

        <v-row
          v-else-if="form.contentType === PageContentType.Embed"
          class="mb-4"
        >
          <v-col cols="12" md="6">
            <label class="text-subtitle-2 mb-2 d-block">
              {{ t("admin.pages.fields.embedUrl.title") }}
            </label>
            <v-text-field
              v-model="form.mediaUrl"
              :placeholder="t('admin.pages.fields.embedUrl.placeholder')"
              outlined
            />
          </v-col>

          <v-col v-if="form.mediaUrl" cols="12" md="6">
            <label class="text-subtitle-2 mb-2 d-block">
              {{ t("admin.pages.fields.preview.title") }}
            </label>
            <div class="embed-preview-frame">
              <!-- Mirrors LessonPlayer.vue's Embed render exactly (a raw
                     iframe pointed at mediaUrl, no URL rewriting) so this
                     preview matches what students will actually see -->
              <iframe
                :src="form.mediaUrl"
                :title="form.title"
                allowfullscreen
              />
            </div>
          </v-col>
        </v-row>

        <v-row v-if="usesGenericContentEditor" class="mb-4">
          <v-col cols="12">
            <label class="text-subtitle-2 mb-2">
              {{ t("admin.pages.fields.content.title") }}
            </label>

            <template v-if="contentReady">
              <template v-if="form.isImported">
                <v-alert
                  type="info"
                  variant="tonal"
                  density="compact"
                  class="mb-2"
                >
                  {{ t("admin.pages.fields.content.importedNotice") }}
                </v-alert>
                <v-btn
                  color="warning"
                  variant="tonal"
                  size="small"
                  prepend-icon="bx-edit-alt"
                  class="mb-4"
                  @click="showConvertConfirm = true"
                >
                  {{ t("admin.pages.actions.convertToEditable") }}
                </v-btn>
                <label class="text-subtitle-2 mb-2 d-block">
                  {{ t("admin.pages.fields.content.richPreview") }}
                </label>
                <iframe
                  :srcdoc="form.content"
                  sandbox="allow-same-origin"
                  class="imported-content-preview"
                  :title="form.title"
                />
                <label class="text-subtitle-2 mt-4 mb-2 d-block">
                  {{ t("admin.pages.fields.content.rawHtml") }}
                </label>
                <v-textarea
                  v-model="form.content"
                  variant="outlined"
                  rows="10"
                  no-resize
                  spellcheck="false"
                  class="raw-html-editor"
                />
              </template>

              <QuillyEditor v-else v-model="form.content" />
            </template>
          </v-col>
        </v-row>

        <v-row class="mt-4">
          <v-col cols="12" class="d-flex gap-2">
            <v-btn type="submit" color="primary" :loading="loading">
              {{ isEdit ? t("shared.save") : t("shared.create") }}
            </v-btn>
            <v-btn :to="backTo" variant="outlined">
              {{ t("shared.cancel") }}
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>

  <confirm-dialog
    v-model:show-dialog="showConvertConfirm"
    :title="t('admin.pages.actions.convertToEditableConfirm.title')"
    :message="t('admin.pages.actions.convertToEditableConfirm.message')"
    :confirm-button-text="
      t('admin.pages.actions.convertToEditableConfirm.confirm')
    "
    :cancel-button-text="t('shared.cancel')"
    confirm-button-color="warning"
    @confirm="handleConvertConfirmed"
  />
</template>

<style scoped>
.imported-content-preview {
  width: 100%;
  aspect-ratio: 960 / 540;
  border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
  border-radius: 4px;
  background: #fff;
}

.raw-html-editor :deep(textarea) {
  font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
  font-size: 13px;
}

.media-preview-thumbnail {
  max-width: 100%;
  max-height: 200px;
  border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
  border-radius: 4px;
}

.embed-preview-frame {
  aspect-ratio: 16 / 9;
  width: 100%;
  max-width: 480px;
  border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
  border-radius: 4px;
  overflow: hidden;
}

.embed-preview-frame iframe {
  width: 100%;
  height: 100%;
  border: 0;
}
</style>
