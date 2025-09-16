<script setup lang="ts">
import { useDateFormat } from "@vueuse/core";

import { User } from "@/models/user";
import { Notify } from "@/stores/notification";
import { UserForm } from "@/views/admin/users";

import { useUserStore } from "@/stores/user";
import { Result } from "@/primitives/result";
const { t } = useI18n();

const userStore = useUserStore();
const { loading } = storeToRefs(userStore);

const route = useRoute();
const router = useRouter();

const pageTitle: ComputedRef<string> = computed(() =>
  t(route.meta.title as string),
);

const DEFAULT_USER: User = {
  id: "",
  firstName: "",
  lastName: "",
  phoneNumber: "",
  email: "",
  language: 1,
  gender: 0,
  birthday: useDateFormat(new Date(), "YYYY-MM-DD").value,
  placeOfBirth: "",
  isActive: true,
  twoFactorEnabled: false,
};

const user = ref<User>(DEFAULT_USER);

onMounted(async () => {
  const { id } = route.params;
  if (id) {
    const response = await userStore.getById(id as string);
    if (response?.succeeded && response.data) {
      user.value = response.data;
      user.value.birthday = useDateFormat(
        user.value.birthday,
        "YYYY-MM-DD",
      ).value;
    }
  }
});

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.users"),
    to: { path: "/admin/users" },
  },
  {
    title: pageTitle.value,
    disabled: true,
  },
]);

const handleSubmit = async (values: User) => {
  const response: Result<User> =
    route.name === "user-create"
      ? await userStore.create(values)
      : await userStore.update(values);

  if (response.succeeded) {
    Notify.success(
      t(
        `admin.users.notifications.${route.name === "user-create" ? "created" : "updated"}`,
      ),
    );
    router.push({ name: "user-list" });
  } else {
    Notify.error(
      t(
        `admin.users.notifications.${route.name === "user-create" ? "createFailed" : "updateFailed"}`,
      ),
    );
  }
};
</script>
<template>
  <breadcrumb :items="breadcrumbs" />

  <user-form
    :user="user"
    :loading="loading"
    :route-name="route.name"
    :page-title="pageTitle"
    @submit="handleSubmit"
  />
</template>
