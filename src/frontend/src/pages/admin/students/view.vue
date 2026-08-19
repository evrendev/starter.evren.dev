<script setup lang="ts">
import { formatDistanceToNow, format } from "date-fns";
import { enUS, tr, de } from "date-fns/locale";

import { useStudentStore } from "@/stores/student";
import { useCourseStore } from "@/stores/course";
import { Notify } from "@/stores/notification";

const { t, locale } = useI18n();
const route = useRoute();

const studentStore = useStudentStore();
const { student, loading } = storeToRefs(studentStore);

const courseStore = useCourseStore();

const dateFnsLocales = { en: enUS, tr, de } as const;
const currentDateFnsLocale = computed(
  () => dateFnsLocales[locale.value as keyof typeof dateFnsLocales] ?? enUS,
);

const enrolledAgo = (dateStr: string) =>
  formatDistanceToNow(new Date(dateStr), {
    addSuffix: true,
    locale: currentDateFnsLocale.value,
  });

const formatDate = (dateStr?: string | null) =>
  dateStr
    ? format(new Date(dateStr), "PPp", { locale: currentDateFnsLocale.value })
    : "-";

const loadStudent = async () => {
  const { id } = route.params;
  if (id) {
    const result = await studentStore.getDetail(id as string);
    if (!result.succeeded) {
      Notify.error(t("admin.students.notifications.loadFailed"));
    }
  }
};

onMounted(async () => {
  await loadStudent();
});

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { path: "/admin" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.students"),
    to: { path: "/admin/students" },
  },
  {
    title: student.value?.fullName || student.value?.email || "",
    disabled: true,
  },
]);

// --- Toggle status ---
const handleToggleStatus = async () => {
  if (!student.value) return;

  const response = await studentStore.toggleStatus(
    student.value.userId,
    !student.value.isActive,
  );

  if (response.succeeded) {
    Notify.success(
      t(
        student.value.isActive
          ? "admin.students.notifications.deactivated"
          : "admin.students.notifications.activated",
      ),
    );
  } else {
    Notify.error(t("admin.students.notifications.toggleStatusFailed"));
  }
};

// --- Unenroll ---
const unenrollCourseId = ref<string | null>(null);
const showUnenrollConfirm = ref(false);

const askUnenroll = (courseId: string) => {
  unenrollCourseId.value = courseId;
  showUnenrollConfirm.value = true;
};

const confirmUnenroll = async () => {
  if (!student.value || !unenrollCourseId.value) return;

  const response = await studentStore.unenrollStudent(
    student.value.userId,
    unenrollCourseId.value,
  );

  if (response.succeeded) {
    Notify.success(t("admin.students.notifications.unenrolled"));
    await loadStudent();
  } else {
    Notify.error(t("admin.students.notifications.unenrollFailed"));
  }
  unenrollCourseId.value = null;
};

// --- Enroll in course dialog ---
const showEnrollDialog = ref(false);
const selectedCourseId = ref<string | null>(null);
const enrolling = ref(false);

const availableCourses = computed(() => {
  const enrolledIds = new Set(
    (student.value?.enrollments ?? []).map((e) => e.courseId),
  );
  return courseStore.items.filter((c) => !enrolledIds.has(c.id));
});

const openEnrollDialog = async () => {
  selectedCourseId.value = null;
  await courseStore.getAllItems();
  showEnrollDialog.value = true;
};

const confirmEnroll = async () => {
  if (!student.value || !selectedCourseId.value) return;

  enrolling.value = true;
  try {
    const response = await studentStore.enrollStudent(
      student.value.userId,
      selectedCourseId.value,
    );

    if (response.succeeded) {
      Notify.success(t("admin.students.notifications.enrolled"));
      showEnrollDialog.value = false;
      await loadStudent();
    } else {
      Notify.error(t("admin.students.notifications.enrollFailed"));
    }
  } finally {
    enrolling.value = false;
  }
};

const statusColor = (status: string) => {
  switch (status) {
    case "Captured":
      return "success";
    case "Failed":
    case "Cancelled":
      return "error";
    case "Approved":
      return "info";
    default:
      return "secondary";
  }
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />

  <v-progress-circular
    v-if="loading && !student"
    indeterminate
    color="primary"
    class="d-block mx-auto mt-8"
  />

  <template v-else-if="student">
    <v-card elevation="6" class="mt-4">
      <v-card-text class="d-flex align-center justify-space-between flex-wrap gap-4">
        <div class="d-flex align-center gap-4">
          <v-avatar color="primary" variant="tonal" size="56">
            <v-icon icon="bx-user" size="28" />
          </v-avatar>
          <div>
            <div class="text-h6">{{ student.fullName || student.email }}</div>
            <div class="text-body-2 text-medium-emphasis">
              {{ student.email }}
            </div>
          </div>
        </div>

        <div class="d-flex align-center gap-4">
          <v-chip :color="student.isActive ? 'success' : 'error'" variant="tonal">
            {{
              student.isActive
                ? t("admin.students.fields.isActive.active")
                : t("admin.students.fields.isActive.inactive")
            }}
          </v-chip>
          <v-chip
            :color="student.emailConfirmed ? 'success' : 'warning'"
            variant="tonal"
          >
            {{
              student.emailConfirmed
                ? t("admin.students.fields.emailConfirmed.confirmed")
                : t("admin.students.fields.emailConfirmed.unconfirmed")
            }}
          </v-chip>
          <v-btn
            :color="student.isActive ? 'error' : 'success'"
            variant="flat"
            size="small"
            @click="handleToggleStatus"
          >
            {{
              student.isActive
                ? t("admin.students.actions.deactivate")
                : t("admin.students.actions.activate")
            }}
          </v-btn>
        </div>
      </v-card-text>
    </v-card>

    <v-card elevation="6" class="mt-4">
      <v-card-title>
        <v-col>
          <v-toolbar
            :title="t('admin.students.view.enrolledCourses')"
            density="compact"
            color="surface"
          >
            <template v-slot:append>
              <v-btn
                variant="flat"
                size="small"
                color="success"
                prepend-icon="bx-plus"
                :text="t('admin.students.actions.enrollInCourse')"
                @click="openEnrollDialog"
              />
            </template>
          </v-toolbar>
        </v-col>
      </v-card-title>
      <v-card-text>
        <v-table v-if="student.enrollments.length > 0">
          <thead>
            <tr>
              <th>{{ t("admin.students.fields.courseTitle.title") }}</th>
              <th>{{ t("admin.students.fields.enrolledAt.title") }}</th>
              <th>{{ t("admin.students.fields.pricePaid.title") }}</th>
              <th style="min-width: 160px">
                {{ t("admin.students.fields.percentComplete.title") }}
              </th>
              <th class="text-center">{{ t("shared.actions") }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="enrollment in student.enrollments" :key="enrollment.courseId">
              <td>{{ enrollment.courseTitle }}</td>
              <td>{{ enrolledAgo(enrollment.enrolledAt) }}</td>
              <td>{{ enrollment.pricePaid.toFixed(2) }} €</td>
              <td>
                <div class="d-flex align-center gap-2">
                  <v-progress-linear
                    :model-value="enrollment.percentComplete"
                    :color="enrollment.percentComplete >= 100 ? 'success' : 'primary'"
                    height="6"
                    rounded
                    style="max-width: 120px"
                  />
                  <span class="text-caption">{{ enrollment.percentComplete }}%</span>
                </div>
              </td>
              <td class="text-center">
                <v-btn
                  color="error"
                  variant="text"
                  size="small"
                  @click="askUnenroll(enrollment.courseId)"
                >
                  {{ t("admin.students.actions.unenroll") }}
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>
        <p v-else class="text-body-2 text-medium-emphasis">
          {{ t("admin.students.view.noEnrollments") }}
        </p>
      </v-card-text>
    </v-card>

    <v-card elevation="6" class="mt-4">
      <v-card-title>
        <v-col>
          <v-toolbar
            :title="t('admin.students.view.paymentHistory')"
            density="compact"
            color="surface"
          />
        </v-col>
      </v-card-title>
      <v-card-text>
        <v-table v-if="student.payments.length > 0">
          <thead>
            <tr>
              <th>{{ t("admin.students.fields.courseTitle.title") }}</th>
              <th>{{ t("admin.students.fields.amount.title") }}</th>
              <th>{{ t("admin.students.fields.status.title") }}</th>
              <th>{{ t("admin.students.fields.payPalCaptureId.title") }}</th>
              <th>{{ t("admin.students.fields.capturedAt.title") }}</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="payment in student.payments" :key="`${payment.courseId}-${payment.payPalCaptureId}`">
              <td>{{ payment.courseTitle }}</td>
              <td>{{ payment.amount.toFixed(2) }} {{ payment.currency }}</td>
              <td>
                <v-chip :color="statusColor(payment.status)" variant="tonal" size="small">
                  {{ payment.status }}
                </v-chip>
              </td>
              <td>{{ payment.payPalCaptureId || "-" }}</td>
              <td>{{ formatDate(payment.capturedAt) }}</td>
            </tr>
          </tbody>
        </v-table>
        <p v-else class="text-body-2 text-medium-emphasis">
          {{ t("admin.students.view.noPayments") }}
        </p>
      </v-card-text>
    </v-card>
  </template>

  <confirm-dialog
    v-model:show-dialog="showUnenrollConfirm"
    :confirm-button-text="t('shared.confirm')"
    :cancel-button-text="t('shared.cancel')"
    :title="t('admin.students.notifications.unenrollConfirmTitle')"
    :message="t('admin.students.notifications.unenrollConfirmMessage')"
    @confirm="confirmUnenroll"
    @cancel="unenrollCourseId = null"
  />

  <v-dialog v-model="showEnrollDialog" max-width="480">
    <v-card>
      <v-card-title>{{ t("admin.students.actions.enrollInCourse") }}</v-card-title>
      <v-card-text>
        <v-select
          v-model="selectedCourseId"
          :items="availableCourses"
          item-title="title"
          item-value="id"
          variant="outlined"
          :label="t('admin.students.fields.courseTitle.title')"
          :no-data-text="t('admin.students.view.noAvailableCourses')"
        />
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn variant="text" @click="showEnrollDialog = false">
          {{ t("shared.cancel") }}
        </v-btn>
        <v-btn
          color="primary"
          variant="flat"
          :disabled="!selectedCourseId"
          :loading="enrolling"
          @click="confirmEnroll"
        >
          {{ t("shared.confirm") }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
