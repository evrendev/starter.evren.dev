<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { VFileUpload } from "vuetify/labs/VFileUpload";
import { toTypedSchema } from "@vee-validate/yup";
import { mixed, object, string } from "yup";
import { useForm } from "vee-validate";
import { Course } from "@/models/course";
import { Category } from "@/models/category";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    course: Course | null;
    categories: Category[];
    pageTitle: string;
    loading: boolean;
    routeName: RouteRecordNameGeneric;
  }>(),
  {
    course: null,
    categories: () => [],
    pageTitle: "",
    loading: false,
    routeName: "course-view",
  },
);

const schema = toTypedSchema(
  object({
    categoryId: string().required(
      t("admin.courses.fields.categoryId.required"),
    ),
    title: string().required(t("admin.courses.fields.title.required")),
    description: string().required(
      t("admin.courses.fields.description.required"),
    ),
    previewVideoUrl: string()
      .nullable()
      .url(t("admin.courses.fields.previewVideoUrl.invalid")),
    image: mixed<File>().nullable(),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<Course>({
  validationSchema: schema,
});

const readOnly: Ref<boolean> = ref(props.routeName === "course-view");

const [id] = defineField("id");
const [categoryId, categoryIdAttrs] = defineField("categoryId");
const [title, titleAttrs] = defineField("title");
const [description, descriptionAttrs] = defineField("description");
const [introduction] = defineField("introduction");
const [tags] = defineField("tags");
const [published] = defineField("published");
const [previewVideoUrl, previewVideoUrlAttrs] = defineField("previewVideoUrl");
const [image] = defineField("image");

watch(
  () => props.course,
  (courseData) => {
    if (courseData) {
      resetForm({ values: courseData });
    }
  },
  {
    immediate: true,
    deep: true,
  },
);

const emit = defineEmits<{
  (e: "submit", values: Course): void;
}>();

const submit = handleSubmit((values: Course) => {
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
          to: { name: 'course-list' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-form :disabled="readOnly">
        <v-row v-show="routeName !== 'course-create'">
          <v-col cols="12" md="3">
            <label
              for="id"
              class="form-label"
              v-text="t('admin.courses.fields.id.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="id"
              variant="outlined"
              :disabled="true"
              :label="t('admin.courses.fields.id.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="categoryId"
              class="form-label"
              v-text="t('admin.courses.fields.categoryId.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-select
              v-model="categoryId"
              v-bind="categoryIdAttrs"
              variant="outlined"
              :items="categories"
              item-value="id"
              item-title="name"
              :label="t('admin.courses.fields.categoryId.title')"
              :error-messages="errors.categoryId"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="title"
              class="form-label"
              v-text="t('admin.courses.fields.title.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="title"
              v-bind="titleAttrs"
              variant="outlined"
              :label="t('admin.courses.fields.title.title')"
              :error-messages="errors.title"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="introduction"
              v-text="t('admin.courses.fields.introduction.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-textarea
              v-model="introduction"
              variant="outlined"
              rows="2"
              :label="t('admin.courses.fields.introduction.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="description"
              v-text="t('admin.courses.fields.description.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-textarea
              v-model="description"
              v-bind="descriptionAttrs"
              variant="outlined"
              :label="t('admin.courses.fields.description.title')"
              :error-messages="errors.description"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              for="tags"
              class="form-label"
              v-text="t('admin.courses.fields.tags.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-combobox v-model="tags" clearable chips multiple />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="previewVideoUrl"
              v-text="t('admin.courses.fields.previewVideoUrl.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-text-field
              v-model="previewVideoUrl"
              v-bind="previewVideoUrlAttrs"
              variant="outlined"
              :error-messages="errors.previewVideoUrl"
              :label="t('admin.courses.fields.previewVideoUrl.title')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="image"
              v-text="t('admin.courses.fields.image.title')"
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
          <v-col cols="12" md="3">
            <label
              class="form-label"
              for="published"
              v-text="t('admin.courses.fields.published.title')"
            />
          </v-col>
          <v-col cols="12" md="9">
            <v-checkbox
              v-model="published"
              variant="outlined"
              density="compact"
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
