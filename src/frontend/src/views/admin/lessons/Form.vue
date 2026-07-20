<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { VFileUpload } from "vuetify/labs/VFileUpload";
import { toTypedSchema } from "@vee-validate/yup";
import { object, string } from "yup";
import { useForm } from "vee-validate";
import { LessonDetails } from "@/models/lesson";
import { Chapter } from "@/models/chapter";
import { useLessonPageStore } from "@/stores/lessonPage";

import { PreviewSlide } from ".";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    lesson: LessonDetails | null;
    chapters: Chapter[];
    pageTitle: string;
    loading: boolean;
    routeName: RouteRecordNameGeneric;
  }>(),
  {
    lesson: null,
    chapters: () => [],
    pageTitle: "",
    loading: false,
    routeName: "lesson-view",
  },
);

const schema = toTypedSchema(
  object({
    chapterId: string().required(t("admin.lessons.fields.chapterId.required")),
    title: string().required(t("admin.lessons.fields.title.required")),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<LessonDetails>(
  {
    validationSchema: schema,
  },
);

const readOnly: Ref<boolean> = ref(props.routeName === "lesson-view");
const showPreview: Ref<boolean> = ref(false);

const [id] = defineField("id");
const [chapterId, chapterIdAttrs] = defineField("chapterId");
const [title, titleAttrs] = defineField("title");

// Pages section: only meaningful once a Lesson actually exists (has an id) —
// hidden entirely in create mode, since LessonPages are always created against
// an existing lessonId
const isExistingLesson = computed(() => props.routeName !== "lesson-create");

const lessonPageStore = useLessonPageStore();
const { items: lessonPages, loading: lessonPagesLoading } = storeToRefs(lessonPageStore);

watch(
  () => props.lesson,
  (lessonData) => {
    if (lessonData) {
      resetForm({ values: lessonData });

      if (lessonData.id) {
        lessonPageStore.getPaginatedPages(lessonData.id);
      }
    }
  },
  {
    immediate: true,
    deep: true,
  },
);

const emit = defineEmits<{
  (e: "submit", values: LessonDetails): void;
}>();

const submit = handleSubmit((values: LessonDetails) => {
  emit("submit", values);
});
</script>

<template>
  <v-card elevation="6" class="mt-4" :disabled="loading">
    <v-card-title>
      <toolbar
        color="secondary"
        :title="pageTitle"
        :button="{
          icon: 'bx-chevron-left',
          text: t('shared.back'),
          to: { name: 'lesson-list' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-form :disabled="readOnly">
        <v-row v-show="routeName !== 'lesson-create'">
          <v-col cols="12" md="3">
            <label
              for="id"
              class="form-label"
              v-text="t('admin.lessons.fields.id.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="id"
              variant="outlined"
              :disabled="true"
              :label="t('admin.lessons.fields.id.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="chapterId"
              class="form-label"
              v-text="t('admin.lessons.fields.chapterId.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-select
              v-model="chapterId"
              v-bind="chapterIdAttrs"
              variant="outlined"
              item-value="id"
              item-title="title"
              :items="chapters"
              :label="t('admin.lessons.fields.chapterId.title')"
              :error-messages="errors.chapterId"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="title"
              class="form-label"
              v-text="t('admin.lessons.fields.title.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="title"
              v-bind="titleAttrs"
              variant="outlined"
              :label="t('admin.lessons.fields.title.title')"
              :error-messages="errors.title"
            />
          </v-col>
        </v-row>
        <v-divider class="mt-16 mb-4"></v-divider>
        <v-row>
          <v-col cols="12" class="d-inline-flex ga-2">
            <v-btn
              color="primary"
              variant="flat"
              size="small"
              prepend-icon="bx-save"
              v-if="!readOnly"
              @click="submit"
            >
              {{ t("shared.save") }}
            </v-btn>
            <v-btn
              color="warning"
              size="small"
              variant="flat"
              prepend-icon="bx-show"
              @click="showPreview = true"
            >
              {{ t("shared.preview") }}
            </v-btn>
            <v-btn
              color="info"
              size="small"
              variant="flat"
              prepend-icon="bx-lock-open-alt"
              v-if="readOnly"
              @click="readOnly = false"
            >
              {{ t("shared.enableEdit") }}
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>

  <v-card v-if="isExistingLesson && lesson" elevation="6" class="mt-4">
    <v-card-title class="d-flex justify-space-between align-center">
      <span>{{ t("admin.lessons.view.pages.title") }}</span>
      <v-btn
        :to="{ name: 'lesson-page-create', params: { lessonId: lesson.id } }"
        color="primary"
        size="small"
        prepend-icon="bx-plus"
      >
        {{ t("admin.lessons.view.pages.addPage") }}
      </v-btn>
    </v-card-title>
    <v-card-text>
      <v-progress-circular
        v-if="lessonPagesLoading"
        indeterminate
        color="primary"
        size="24"
      />
      <v-alert
        v-else-if="lessonPages.length === 0"
        type="info"
        variant="tonal"
      >
        {{ t("admin.lessons.view.pages.noPages") }}
      </v-alert>
      <v-list v-else lines="two">
        <v-list-item
          v-for="page in lessonPages"
          :key="page.id"
          :to="{ name: 'lesson-page-edit', params: { id: page.id } }"
        >
          <v-list-item-title>
            {{ page.order }}. {{ page.title }}
            <v-chip
              v-if="page.needsReview"
              size="x-small"
              color="error"
              variant="flat"
              class="ml-2"
            >
              {{ t("admin.lessons.view.pages.badges.needsReview") }}
            </v-chip>
            <v-chip
              v-if="page.isImported"
              size="x-small"
              color="info"
              variant="flat"
              class="ml-2"
            >
              {{ t("admin.lessons.view.pages.badges.isImported") }}
            </v-chip>
          </v-list-item-title>
          <v-list-item-subtitle>
            {{
              page.contentType
                ? t(`admin.lessonpages.fields.contentType.options.${page.contentType.toLowerCase()}`)
                : "-"
            }}
          </v-list-item-subtitle>
        </v-list-item>
      </v-list>
    </v-card-text>
  </v-card>

  <modal-window
    :show-modal="showPreview"
    :title="lesson?.title || t('shared.preview')"
    width="960"
  >
    <template #content>
      <preview-slide :lesson="lesson" />
    </template>

    <template #action-buttons>
      <v-btn
        color="primary"
        variant="flat"
        size="small"
        prepend-icon="bx-x"
        @click="showPreview = false"
      >
        {{ t("shared.close") }}
      </v-btn>
    </template>
  </modal-window>
</template>

<style scoped type="scss">
:deep(label) {
  &.form-label {
    font-weight: 600;

    &::after {
      content: ":";
    }
  }
}
</style>
