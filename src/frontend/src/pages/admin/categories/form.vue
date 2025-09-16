<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { CategoryForm } from "@/views/admin/categories";

import { useCategoryStore } from "@/stores/category";
import { Category } from "@/models/category";
import { Result } from "@/primitives/result";
const { t } = useI18n();

const categoryStore = useCategoryStore();
const { loading } = storeToRefs(categoryStore);

const route = useRoute();
const router = useRouter();

const pageTitle: ComputedRef<string> = computed(() =>
  t(route.meta.title as string),
);

const category = ref<Category | null>(null);

onMounted(async () => {
  const { id } = route.params;
  if (id) {
    const response: Result<Category | null> = await categoryStore.getById(
      id as string,
    );

    category.value = response?.data ?? null;
  }
});

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.categories"),
    to: { path: "/admin/categories" },
  },
  {
    title: pageTitle.value,
    disabled: true,
  },
]);

const handleSubmit = async (values: Category) => {
  const response: Result<Category> =
    route.name === "category-create"
      ? await categoryStore.create(values)
      : await categoryStore.update(values);

  if (response.succeeded) {
    Notify.success(
      t(
        `admin.categories.notifications.${route.name === "category-create" ? "created" : "updated"}`,
      ),
    );
    router.push({ name: "category-list" });
  } else {
    Notify.error(
      t(
        `admin.categories.notifications.${route.name === "category-create" ? "createFailed" : "updateFailed"}`,
      ),
    );
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <category-form
    :category="category"
    :loading="loading"
    :route-name="route.name"
    :page-title="pageTitle"
    @submit="handleSubmit"
  />
</template>
