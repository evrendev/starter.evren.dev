<script lang="ts" setup>
import { useAuthStore } from "@/stores/auth";
const authStore = useAuthStore();
const { t } = useI18n();
import VerticalNavSectionTitle from "./VerticalNavSectionTitle.vue";
import VerticalNavGroup from "./VerticalNavGroup.vue";
import VerticalNavLink from "./VerticalNavLink.vue";
import { Permissions } from "@/models/user";
</script>

<template>
  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.dashboard'),
      icon: 'bx-home',
      to: { name: 'dashboard' },
    }"
    v-show="authStore.hasPermission(Permissions.DashboardView)"
  />

  <vertical-nav-section-title
    :item="{
      heading: t('admin.components.sidebar.others'),
    }"
  />

  <vertical-nav-group
    :item="{
      title: t('admin.components.sidebar.admin'),
      icon: 'bx-user',
    }"
    v-show="
      authStore.hasPermission([
        Permissions.TenantView,
        Permissions.RoleView,
        Permissions.UserView,
      ])
    "
  >
    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.tenants'),
        to: { name: 'tenant-list' },
      }"
      v-show="authStore.hasPermission(Permissions.TenantView)"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.roles'),
        to: { name: 'role-list' },
      }"
      v-show="authStore.hasPermission(Permissions.RoleView)"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.users'),
        to: { name: 'users' },
      }"
      v-show="authStore.hasPermission(Permissions.UserView)"
    />
  </vertical-nav-group>

  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.documentation'),
      icon: 'bx-file',
      href: '/documentation',
      target: '_blank',
    }"
  />
</template>
