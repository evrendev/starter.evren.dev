<script setup lang="ts">
import { DataTable, StudentFilter } from "@/views/admin/students";
import ElevatedCard from "@/components/shared/ElevatedCard.vue";

import { Notify } from "@/stores/notification";
import { useStudentStore } from "@/stores/student";
import { AdvancedFilters, Filters } from "@/types/requests/student";
import { StudentSummary } from "@/models/student";

const studentStore = useStudentStore();
const { loading, total, itemsPerPage, items, summaryStats } =
  storeToRefs(studentStore);

const { t } = useI18n();
const route = useRoute();

const headers = computed(() => [
  ...([
    {
      title: t("admin.students.fields.isActive.title"),
      key: "isActive",
      align: "center",
      sortable: false,
      width: "64px",
    },
    {
      title: t("admin.students.fields.fullName.title"),
      key: "fullName",
      sortable: false,
    },
    {
      title: t("admin.students.fields.email.title"),
      key: "email",
      sortable: false,
    },
    {
      title: t("admin.students.fields.enrolledCourseCount.title"),
      key: "enrolledCourseCount",
      align: "center",
      sortable: false,
    },
    {
      title: t("admin.students.fields.totalPaid.title"),
      key: "totalPaid",
      align: "end",
      sortable: false,
    },
    {
      title: t("admin.students.fields.averageCompletionPercent.title"),
      key: "averageCompletionPercent",
      align: "center",
      sortable: false,
    },
    {
      title: t("shared.actions"),
      key: "actions",
      align: "center",
      sortable: false,
      width: "100px",
    },
  ] as const),
]);

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { name: "dashboard" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.students"),
    to: { path: "/admin/students" },
  },
  {
    title: t(route.meta.title as string),
    disabled: true,
  },
]);

onMounted(async () => {
  await studentStore.getSummaryStats();
});

const handleToggleStatus = async (student: StudentSummary) => {
  const response = await studentStore.toggleStatus(
    student.userId,
    !student.isActive,
  );

  if (response.succeeded) {
    Notify.success(
      t(
        student.isActive
          ? "admin.students.notifications.deactivated"
          : "admin.students.notifications.activated",
      ),
    );
  } else {
    Notify.error(t("admin.students.notifications.toggleStatusFailed"));
  }
};

const handleUpdateFilters = async (filters: AdvancedFilters) => {
  studentStore.setFilters(filters);
  await studentStore.getPaginatedItems();
};

const handleResetFilters = async () => {
  studentStore.resetFilters();
  await studentStore.getPaginatedItems();
};

const getPaginatedItems = async (options: Filters) => {
  studentStore.setFilters(options);
  await studentStore.getPaginatedItems();
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />

  <v-row class="mb-2 mt-1">
    <v-col cols="12" sm="4">
      <ElevatedCard class="d-flex align-center pa-4">
        <v-avatar color="primary" variant="tonal" size="40" class="me-4">
          <v-icon icon="bx-group" />
        </v-avatar>
        <div>
          <div class="text-h5 font-weight-bold">{{ total }}</div>
          <div class="text-caption text-medium-emphasis">
            {{ t("admin.students.stats.totalStudents") }}
          </div>
        </div>
      </ElevatedCard>
    </v-col>
    <v-col cols="12" sm="4">
      <ElevatedCard class="d-flex align-center pa-4">
        <v-avatar color="success" variant="tonal" size="40" class="me-4">
          <v-icon icon="bx-euro" />
        </v-avatar>
        <div>
          <div class="text-h5 font-weight-bold">
            {{ (summaryStats?.totalRevenue ?? 0).toFixed(2) }} €
          </div>
          <div class="text-caption text-medium-emphasis">
            {{ t("admin.students.stats.totalRevenue") }}
          </div>
        </div>
      </ElevatedCard>
    </v-col>
    <v-col cols="12" sm="4">
      <ElevatedCard class="d-flex align-center pa-4">
        <v-avatar color="warning" variant="tonal" size="40" class="me-4">
          <v-icon icon="bx-trending-up" />
        </v-avatar>
        <div>
          <div class="text-h5 font-weight-bold">
            {{ Math.round(summaryStats?.averageCompletionPercent ?? 0) }}%
          </div>
          <div class="text-caption text-medium-emphasis">
            {{ t("admin.students.stats.averageCompletion") }}
          </div>
        </div>
      </ElevatedCard>
    </v-col>
  </v-row>

  <student-filter
    :page-title="t('shared.filters.title')"
    :disabled="loading"
    @submit="handleUpdateFilters"
    @reset="handleResetFilters"
  />
  <data-table
    :items-per-page="itemsPerPage"
    :items="items"
    :total="total"
    :loading="loading"
    :headers="headers"
    @toggle-status="handleToggleStatus"
    @update:options="getPaginatedItems"
    item-value="userId"
  />
</template>
