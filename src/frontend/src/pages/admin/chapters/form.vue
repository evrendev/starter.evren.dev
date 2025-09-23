<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { ChapterForm } from "@/views/admin/chapters";

import { useCourseStore } from "@/stores/course";
import { useChapterStore } from "@/stores/chapter";
import { Chapter } from "@/models/chapter";
import { Result } from "@/primitives/result";
const { t } = useI18n();

const courseStore = useCourseStore();
const { items: courses } = storeToRefs(courseStore);

const chapterStore = useChapterStore();
const { loading, chapter } = storeToRefs(chapterStore);

const route = useRoute();
const router = useRouter();

watch(
  () => route.params.id,
  async (id) => {
    if (id) {
      await courseStore.getAllItems();
      await chapterStore.getById(id as string);
    } else {
      chapterStore.$reset();
    }
  },
  {
    immediate: true,
  },
);

const pageTitle: ComputedRef<string> = computed(() =>
  t(route.meta.title as string),
);

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.chapters"),
    to: { path: "/admin/chapters" },
  },
  {
    title: pageTitle.value,
    disabled: true,
  },
]);

const handleSubmit = async (values: Chapter) => {
  const response: Result<Chapter> =
    route.name === "chapter-create"
      ? await chapterStore.create(values)
      : await chapterStore.update(values);

  if (response.succeeded) {
    Notify.success(
      t(
        `admin.chapters.notifications.${route.name === "chapter-create" ? "created" : "updated"}`,
      ),
    );
    router.push({ name: "chapter-list" });
  } else {
    Notify.error(
      t(
        `admin.chapters.notifications.${route.name === "chapter-create" ? "createFailed" : "updateFailed"}`,
      ),
    );
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <chapter-form
    :chapter="chapter"
    :courses="courses"
    :loading="loading"
    :route-name="route.name"
    :page-title="pageTitle"
    @submit="handleSubmit"
  />
</template>
