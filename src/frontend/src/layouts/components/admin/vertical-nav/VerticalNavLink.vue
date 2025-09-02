<script lang="ts" setup>
import type { NavLink } from "@/layouts/types";

defineProps<{
  item: NavLink;
}>();
</script>

<template>
  <li class="nav-link" :class="{ disabled: item.disable }">
    <Component
      :is="item.to ? 'router-link' : 'a'"
      :to="item.to"
      :href="item.href"
      :target="item.target"
    >
      <VIcon :icon="item.icon || 'bxs-circle'" class="nav-item-icon" />

      <span class="nav-item-title">
        {{ item.title }}
      </span>

      <span class="nav-item-badge" :class="item.badgeClass">
        {{ item.badgeContent }}
      </span>
    </Component>
  </li>
</template>

<style lang="scss">
.layout-vertical-nav {
  .nav-link a {
    display: flex;
    align-items: center;
    cursor: pointer;
  }
}
</style>
