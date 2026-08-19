<script lang="ts" setup>
import { computed } from "vue";
import { Permissions } from "@/models/user";
const { t } = useI18n();

import { usePersonalStore } from "@/stores/personal";
const personalStore = usePersonalStore();

import { VerticalNavSectionTitle, VerticalNavGroup, VerticalNavLink } from "./";

const BASE_URL = import.meta.env.VITE_APP_BACKEND_BASE_URL;

// Same Create-based check used to gate "Course Management" below — a user
// who can manage the catalog doesn't need the learner-facing "My Learning"
// shortcuts, and a user who can't shouldn't see the admin CRUD screens.
// The two sections are exact complements of one another.
const canManageCourses = computed(() =>
  personalStore.hasPermission([
    Permissions.CategoryCreate,
    Permissions.CourseCreate,
    Permissions.ChapterCreate,
  ]),
);

const canSeeAdminGroup = computed(() =>
  personalStore.hasPermission([
    Permissions.TenantView,
    Permissions.RoleView,
    Permissions.UserView,
    Permissions.StudentView,
  ]),
);

const canSeeHangfire = computed(() =>
  personalStore.hasPermission(Permissions.HangfireView),
);

// "OTHERS" only has these two entries — hide the heading when neither has
// anything to show for the current user, instead of leaving an empty title.
const showOthersHeading = computed(
  () => canSeeAdminGroup.value || canSeeHangfire.value,
);
</script>

<template>
  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.dashboard'),
      icon: 'bx-home',
      to: { name: 'dashboard' },
    }"
    v-show="personalStore.hasPermission(Permissions.DashboardView)"
  />

  <vertical-nav-section-title
    :item="{
      heading: t('admin.components.sidebar.my-learning'),
    }"
    v-show="!canManageCourses"
  />

  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.learning-catalog'),
      icon: 'bx-search-alt',
      to: { name: 'learning-catalog' },
    }"
    v-show="!canManageCourses"
  />

  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.learning-my-courses'),
      icon: 'bx-book-reader',
      to: { name: 'learning-my-courses' },
    }"
    v-show="!canManageCourses"
  />

  <!--
    Gated on Create (not View): Student has View+Search on these resources so
    /learning's API calls work, but that must not surface the admin CRUD
    screens in the sidebar. Create is only granted to Admin/Editor, so it's
    the accurate proxy for "has management access to this resource" (see
    Task O3 bug fix — do not revert to *View here).
  -->
  <vertical-nav-section-title
    :item="{
      heading: t('admin.components.sidebar.course-management'),
    }"
    v-show="canManageCourses"
  />

  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.categories'),
      icon: 'bx-category',
      to: { name: 'category-list' },
    }"
    v-show="personalStore.hasPermission(Permissions.CategoryCreate)"
  />

  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.courses'),
      icon: 'bx-book',
      to: { name: 'course-list' },
    }"
    v-show="personalStore.hasPermission(Permissions.CourseCreate)"
  />

  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.chapters'),
      icon: 'bx-list-ol',
      to: { name: 'chapter-list' },
    }"
    v-show="personalStore.hasPermission(Permissions.ChapterCreate)"
  />

  <vertical-nav-section-title
    :item="{
      heading: t('admin.components.sidebar.others'),
    }"
    v-show="showOthersHeading"
  />

  <vertical-nav-group
    :item="{
      title: t('admin.components.sidebar.admin'),
      icon: 'bx-user',
    }"
    v-show="canSeeAdminGroup"
  >
    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.tenants'),
        to: { name: 'tenant-list' },
      }"
      v-show="personalStore.hasPermission(Permissions.TenantView)"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.roles'),
        to: { name: 'role-list' },
      }"
      v-show="personalStore.hasPermission(Permissions.RoleView)"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.users'),
        to: { name: 'user-list' },
      }"
      v-show="personalStore.hasPermission(Permissions.UserView)"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.students'),
        to: { name: 'student-list' },
      }"
      v-show="personalStore.hasPermission(Permissions.StudentView)"
    />
  </vertical-nav-group>

  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.hangfire'),
      icon: 'bx-task',
      href: `${BASE_URL}/jobs`,
      target: '_blank',
    }"
    v-show="canSeeHangfire"
  />
</template>
