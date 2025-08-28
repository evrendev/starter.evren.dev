<script lang="ts" setup>
import { useAuthStore } from "@/stores/auth";
const authStore = useAuthStore();
const { t } = useI18n();
import VerticalNavSectionTitle from "@/@layouts/components/VerticalNavSectionTitle.vue";
import VerticalNavGroup from "@layouts/components/VerticalNavGroup.vue";
import VerticalNavLink from "@layouts/components/VerticalNavLink.vue";
</script>

<template>
  <vertical-nav-link
    :item="{
      title: t('admin.components.sidebar.dashboard'),
      icon: 'bx-home',
      to: '/admin',
    }"
    v-show="authStore.hasPermission('Permissions.Dashboard.View')"
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
        'Permissions.Tenants.View',
        'Permissions.Roles.View',
        'Permissions.Users.View',
      ])
    "
  >
    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.tenants'),
        to: '/admin/admin/tenants',
      }"
      v-show="authStore.hasPermission('Permissions.Tenants.View')"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.roles'),
        to: '/admin/admin/roles',
      }"
      v-show="authStore.hasPermission('Permissions.Roles.View')"
    />

    <vertical-nav-link
      :item="{
        title: t('admin.components.sidebar.users'),
        to: '/admin/admin/users',
      }"
      v-show="authStore.hasPermission('Permissions.Users.View')"
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
