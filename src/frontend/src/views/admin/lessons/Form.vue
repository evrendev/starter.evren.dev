<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { VFileUpload } from "vuetify/labs/VFileUpload";
import { toTypedSchema } from "@vee-validate/yup";
import { mixed, object, string } from "yup";
import { useForm } from "vee-validate";
import { LessonDetails } from "@/models/lesson";
import { Chapter } from "@/models/chapter";

import { PreviewSlide } from ".";
import QuillyEditor from "@/components/admin/QuillyEditor.vue";

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
    content: string().required(t("admin.lessons.fields.content.required")),
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
const [content, contentAttrs] = defineField("content");

watch(
  () => props.lesson,
  (lessonData) => {
    if (lessonData) {
      resetForm({ values: lessonData });
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
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="content"
              v-text="t('admin.lessons.fields.content.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <quilly-editor
              v-bind="contentAttrs"
              v-model="content"
              :read-only="readOnly"
              :placeholder="t('admin.lessons.fields.content.placeholder')"
            />
            <p v-if="errors.content" class="text-error text-sm mt-1 ml-5">
              {{ errors.content }}
            </p>
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
