<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { toTypedSchema } from "@vee-validate/yup";
import { object, string } from "yup";
import { useForm } from "vee-validate";
import { Chapter } from "@/models/chapter";
import { Course } from "@/models/course";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    chapter: Chapter | null;
    courses: Course[];
    pageTitle: string;
    loading: boolean;
    routeName: RouteRecordNameGeneric;
  }>(),
  {
    chapter: null,
    courses: () => [],
    pageTitle: "",
    loading: false,
    routeName: "chapter-view",
  },
);

const schema = toTypedSchema(
  object({
    courseId: string().required(t("admin.chapters.fields.courseId.required")),
    title: string().required(t("admin.chapters.fields.title.required")),
    description: string().required(
      t("admin.chapters.fields.description.required"),
    ),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<Chapter>({
  validationSchema: schema,
});

const readOnly: Ref<boolean> = ref(props.routeName === "chapter-view");

const [id] = defineField("id");
const [courseId, courseIdAttrs] = defineField("courseId");
const [title, titleAttrs] = defineField("title");
const [description, descriptionAttrs] = defineField("description");

watch(
  () => props.chapter,
  (chapterData) => {
    if (chapterData) {
      resetForm({ values: chapterData });
    }
  },
  {
    immediate: true,
    deep: true,
  },
);

const emit = defineEmits<{
  (e: "submit", values: Chapter): void;
}>();

const submit = handleSubmit((values: Chapter) => {
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
          to: { name: 'chapter-list' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-form :disabled="readOnly">
        <v-row v-show="routeName !== 'chapter-create'">
          <v-col cols="12" md="3">
            <label
              for="id"
              class="form-label"
              v-text="t('admin.chapters.fields.id.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="id"
              variant="outlined"
              :disabled="true"
              :label="t('admin.chapters.fields.id.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="courseId"
              class="form-label"
              v-text="t('admin.chapters.fields.courseId.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-select
              v-model="courseId"
              v-bind="courseIdAttrs"
              variant="outlined"
              :items="courses"
              item-value="id"
              item-title="title"
              :label="t('admin.chapters.fields.courseId.title')"
              :error-messages="errors.courseId"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="title"
              class="form-label"
              v-text="t('admin.chapters.fields.title.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="title"
              v-bind="titleAttrs"
              variant="outlined"
              :label="t('admin.chapters.fields.title.title')"
              :error-messages="errors.title"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="description"
              v-text="t('admin.chapters.fields.description.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-textarea
              v-model="description"
              v-bind="descriptionAttrs"
              variant="outlined"
              :label="t('admin.chapters.fields.description.title')"
              :error-messages="errors.description"
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
