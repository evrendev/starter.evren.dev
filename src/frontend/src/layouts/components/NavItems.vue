<script lang="ts" setup>
import VerticalNavSectionTitle from "@/@layouts/components/VerticalNavSectionTitle.vue";
import VerticalNavGroup from "@layouts/components/VerticalNavGroup.vue";
import VerticalNavLink from "@layouts/components/VerticalNavLink.vue";
</script>

<template>
  <vertical-nav-group
    :item="{
      title: 'Dashboards',
      badgeContent: '5',
      badgeClass: 'bg-error',
      icon: 'bx-home-smile',
    }"
  >
    <vertical-nav-link
      :item="{
        title: 'Analytics',
        to: '/admin/dashboard',
      }"
    />
  </vertical-nav-group>

  <vertical-nav-section-title
    :item="{
      heading: 'Others',
    }"
  />
  <vertical-nav-link
    :item="{
      title: 'Documentation',
      icon: 'bx-file',
      href: '/documentation',
      target: '_blank',
    }"
  />
</template>
