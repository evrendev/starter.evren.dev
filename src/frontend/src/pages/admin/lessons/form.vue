<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { LessonForm } from "@/views/admin/lessons";

import { useChapterStore } from "@/stores/chapter";
import { useLessonStore } from "@/stores/lesson";
import { Lesson } from "@/models/lesson";
import { Result } from "@/primitives/result";
const { t } = useI18n();

const chapterStore = useChapterStore();
const { items: chapters } = storeToRefs(chapterStore);

const lessonStore = useLessonStore();
const { loading, lesson } = storeToRefs(lessonStore);

const route = useRoute();
const router = useRouter();

watch(
  () => route.params.id,
  async (id) => {
    if (id) {
      await chapterStore.getAllItems();
      await lessonStore.getById(id as string);
    } else {
      lessonStore.$reset();
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
    title: t("admin.components.breadcrumbs.admin.lessons"),
    to: { path: "/admin/lessons" },
  },
  {
    title: pageTitle.value,
    disabled: true,
  },
]);

const handleSubmit = async (values: Lesson) => {
  const response: Result<Lesson> =
    route.name === "lesson-create"
      ? await lessonStore.create(values)
      : await lessonStore.update(values);

  if (response.succeeded) {
    Notify.success(
      t(
        `admin.lessons.notifications.${route.name === "lesson-create" ? "created" : "updated"}`,
      ),
    );
    router.push({ name: "lesson-list" });
  } else {
    Notify.error(
      t(
        `admin.lessons.notifications.${route.name === "lesson-create" ? "createFailed" : "updateFailed"}`,
      ),
    );
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <lesson-form
    :lesson="lesson"
    :chapters="chapters"
    :loading="loading"
    :route-name="route.name"
    :page-title="pageTitle"
    @submit="handleSubmit"
  />
</template>
