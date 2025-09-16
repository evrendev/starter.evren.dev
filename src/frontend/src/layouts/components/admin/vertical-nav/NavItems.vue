<script lang="ts" setup>
import { Permissions } from "@/models/user";
const { t } = useI18n();

import { usePersonalStore } from "@/stores/personal";
const personalStore = usePersonalStore();

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
    v-show="personalStore.hasPermission(Permissions.DashboardView)"
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
      personalStore.hasPermission([
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
