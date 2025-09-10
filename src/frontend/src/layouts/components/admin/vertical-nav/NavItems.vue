<script lang="ts" setup>
import { Permissions } from "@/models/user";
const { t } = useI18n();

import { useProfileStore } from "@/stores/profile";
const profileStore = useProfileStore();

import VerticalNavSectionTitle from "./VerticalNavSectionTitle.vue";
import VerticalNavGroup from "./VerticalNavGroup.vue";
import VerticalNavLink from "./VerticalNavLink.vue";
</script>

<template>
  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.dashboard'),
      icon: 'bx-home',
      to: { name: 'dashboard' },
    }"
    v-show="profileStore.hasPermission(Permissions.DashboardView)"
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
      profileStore.hasPermission([
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
      v-show="profileStore.hasPermission(Permissions.TenantView)"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.roles'),
        to: { name: 'role-list' },
      }"
      v-show="profileStore.hasPermission(Permissions.RoleView)"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.users'),
        to: { name: 'users' },
      }"
      v-show="profileStore.hasPermission(Permissions.UserView)"
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
