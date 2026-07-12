<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { useLessonPageStore } from "@/stores/lessonPage";
import { useLessonStore } from "@/stores/lesson";
import { LessonPageContentType } from "@/models/lessonPage";
import { CreateLessonPageRequest, UpdateLessonPageRequest } from "@/types/requests/lessonPage";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const lessonPageStore = useLessonPageStore();
const lessonStore = useLessonStore();
const { lessonPage, loading } = storeToRefs(lessonPageStore);

const lessonId = computed(() => route.params.lessonId as string);
const pageId = computed(() => route.params.id as string);
const isEdit = computed(() => !!pageId.value);

// Form state
const form = reactive({
  title: "",
  content: "",
  contentType: LessonPageContentType.Text,
  order: 0,
  mediaUrl: "",
});

const contentTypeOptions = Object.values(LessonPageContentType);

onMounted(async () => {
  if (isEdit.value && pageId.value) {
    const result = await lessonPageStore.getPageById(pageId.value);
    if (result.succeeded && lessonPage.value) {
      form.title = lessonPage.value.title;
      form.content = lessonPage.value.content || "";
      form.contentType = (lessonPage.value.contentType as LessonPageContentType) || LessonPageContentType.Text;
      form.order = lessonPage.value.order || 0;
      form.mediaUrl = lessonPage.value.mediaUrl || "";
    }
  }
});

const pageTitle = computed(() => {
  if (isEdit.value) {
    return t("admin.lessonpages.edit.title");
  }
  return t("admin.lessonpages.create.title");
});

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { name: "dashboard" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.lessons"),
    to: { path: "/admin/lessons" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.lessonpages"),
    to: { path: `/admin/lessons/${lessonId.value}/pages` },
  },
  {
    title: isEdit.value ? t("shared.edit") : t("shared.create"),
    disabled: true,
  },
]);

const handleSubmit = async () => {
  try {
    let response;

    if (isEdit.value && pageId.value) {
      const payload: UpdateLessonPageRequest = {
        id: pageId.value,
        lessonId: lessonId.value,
        title: form.title,
        content: form.content,
        contentType: form.contentType,
        order: form.order,
        mediaUrl: form.mediaUrl || undefined,
      };
      response = await lessonPageStore.updatePage(pageId.value, payload);
    } else {
      const payload: CreateLessonPageRequest = {
        lessonId: lessonId.value,
        title: form.title,
        content: form.content,
        contentType: form.contentType,
        order: form.order,
        mediaUrl: form.mediaUrl || undefined,
      };
      response = await lessonPageStore.createPage(payload);
    }

    if (response.succeeded) {
      const msg = isEdit.value
        ? t("admin.lessonpages.notifications.updated")
        : t("admin.lessonpages.notifications.created");
      Notify.success(msg);
      await router.push({
        path: `/admin/lessons/${lessonId.value}/pages`,
      });
    } else {
      Notify.error(t("admin.lessonpages.notifications.saveFailed"));
    }
  } catch (error) {
    Notify.error(t("shared.unexpectedError"));
  }
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />

  <v-container>
    <v-card>
      <v-card-title>{{ pageTitle }}</v-card-title>

      <v-card-text>
        <v-form @submit.prevent="handleSubmit">
          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="form.title"
                :label="t('admin.lessonpages.fields.title.title')"
                :placeholder="t('admin.lessonpages.fields.title.placeholder')"
                outlined
                required
              />
            </v-col>

            <v-col cols="12" md="6">
              <v-select
                v-model="form.contentType"
                :items="contentTypeOptions"
                :label="t('admin.lessonpages.fields.contentType.title')"
                outlined
              />
            </v-col>
          </v-row>

          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model.number="form.order"
                :label="t('admin.lessonpages.fields.order.title')"
                type="number"
                outlined
              />
            </v-col>

            <v-col cols="12" md="6">
              <v-text-field
                v-model="form.mediaUrl"
                :label="t('admin.lessonpages.fields.mediaUrl.title')"
                :placeholder="t('admin.lessonpages.fields.mediaUrl.placeholder')"
                outlined
              />
            </v-col>
          </v-row>

          <v-row>
            <v-col cols="12">
              <label class="text-subtitle-2 mb-2">
                {{ t("admin.lessonpages.fields.content.title") }}
              </label>
              <QuillEditor
                v-model:content="form.content"
                content-type="html"
                theme="snow"
              />
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12" class="d-flex gap-2">
              <v-btn
                type="submit"
                color="primary"
                :loading="loading"
              >
                {{ isEdit ? t("shared.save") : t("shared.create") }}
              </v-btn>
              <v-btn
                :to="{ path: `/admin/lessons/${lessonId}/pages` }"
                variant="outlined"
              >
                {{ t("shared.cancel") }}
              </v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
    </v-card>
  </v-container>
</template>
