<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { useLessonPageStore } from "@/stores/lessonPage";
import { useLessonStore } from "@/stores/lesson";
import { LessonPageContentType } from "@/models/lessonPage";
import { AdvancedFilters } from "@/types/requests/lessonPage";

const lessonPageStore = useLessonPageStore();
const { loading, items } = storeToRefs(lessonPageStore);

const lessonStore = useLessonStore();
const { items: lessons } = storeToRefs(lessonStore);

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const lessonId = computed(() => route.params.lessonId as string);

onMounted(async () => {
  await lessonStore.getAllItems();
  if (lessonId.value) {
    await lessonPageStore.getPaginatedPages(lessonId.value);
  }
});

const headers = computed(() => [
  {
    title: t("admin.lessonpages.fields.order.title"),
    key: "order",
    sortable: false,
    width: "80px",
  } as const,
  {
    title: t("admin.lessonpages.fields.title.title"),
    key: "title",
    sortable: false,
    width: "200px",
  } as const,
  {
    title: t("admin.lessonpages.fields.contentType.title"),
    key: "contentType",
    sortable: false,
    width: "120px",
  } as const,
  {
    title: t("shared.actions"),
    key: "actions",
    align: "center" as const,
    sortable: false,
    width: "120px",
  } as const,
]);

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
]);

const handleDelete = async (id: string) => {
  const response = await lessonPageStore.deletePage(id);

  if (response.succeeded) {
    Notify.success(t("admin.lessonpages.notifications.deleted"));
    await lessonPageStore.getPaginatedPages(lessonId.value);
  } else {
    Notify.error(t("admin.lessonpages.notifications.deleteFailed"));
  }
};

const pageTitle = computed(() => t(route.meta.title as string));
</script>

<template>
  <breadcrumb :items="breadcrumbs" />

  <v-container>
    <v-card>
      <v-card-title class="d-flex justify-space-between align-center">
        <span>{{ pageTitle }}</span>
        <v-btn
          :to="{ name: 'lesson-page-create', params: { lessonId } }"
          color="primary"
          size="small"
          prepend-icon="mdi-plus"
        >
          {{ t("shared.create") }}
        </v-btn>
      </v-card-title>

      <v-data-table
        :headers="headers"
        :items="items"
        :loading="loading"
        class="elevation-1"
      >
        <template #item.contentType="{ item }">
          <v-chip size="small" :label="true">
            {{ item.contentType || "-" }}
          </v-chip>
        </template>

        <template #item.actions="{ item }">
          <div class="d-flex gap-2">
            <v-btn
              :to="{ name: 'lesson-page-edit', params: { id: item.id, lessonId } }"
              icon="mdi-pencil"
              size="x-small"
              color="info"
              variant="text"
            />
            <v-btn
              icon="mdi-delete"
              size="x-small"
              color="error"
              variant="text"
              @click="handleDelete(item.id)"
            />
          </div>
        </template>
      </v-data-table>
    </v-card>
  </v-container>
</template>
