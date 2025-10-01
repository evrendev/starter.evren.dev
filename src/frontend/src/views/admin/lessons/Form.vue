<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { VFileUpload } from "vuetify/labs/VFileUpload";
import { toTypedSchema } from "@vee-validate/yup";
import { mixed, object, string } from "yup";
import { useForm } from "vee-validate";
import { Lesson } from "@/models/lesson";
import { Chapter } from "@/models/chapter";

import QuillyEditor from "@/components/admin/QuillyEditor.vue";
import { error } from "console";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    lesson: Lesson | null;
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
    description: string().required(
      t("admin.lessons.fields.description.required"),
    ),
    content: string().required(t("admin.lessons.fields.content.required")),
    image: mixed<File>().nullable(),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<Lesson>({
  validationSchema: schema,
});

const readOnly: Ref<boolean> = ref(props.routeName === "lesson-view");

const [id] = defineField("id");
const [chapterId, chapterIdAttrs] = defineField("chapterId");
const [title, titleAttrs] = defineField("title");
const [description, descriptionAttrs] = defineField("description");
const [content, contentAttrs] = defineField("content");
const [notes] = defineField("notes");
const [image] = defineField("image");

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
  (e: "submit", values: Lesson): void;
}>();

const submit = handleSubmit((values: Lesson) => {
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
              for="description"
              v-text="t('admin.lessons.fields.description.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-textarea
              v-model="description"
              v-bind="descriptionAttrs"
              rows="2"
              variant="outlined"
              :label="t('admin.lessons.fields.description.title')"
              :error-messages="errors.description"
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
        <v-row class="mt-15">
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="notes"
              v-text="t('admin.lessons.fields.notes.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-textarea
              v-model="notes"
              variant="outlined"
              rows="2"
              :label="t('admin.lessons.fields.notes.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="image"
              v-text="t('admin.lessons.fields.image.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-file-upload
              v-model="image"
              icon="bx-upload"
              clearable
              scrim="primary"
              density="comfortable"
              :show-size="true"
              :disabled="readOnly"
              :multiple="false"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
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
