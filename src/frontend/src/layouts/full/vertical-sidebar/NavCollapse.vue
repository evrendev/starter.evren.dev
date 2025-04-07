<script setup>
import { NavItem, IconSet } from "./";
import { useI18n } from "vue-i18n";
const { t } = useI18n();

const props = defineProps({ item: Object, level: Number });
</script>

<template>
  <!-- ---------------------------------------------- -->
  <!---Item Childern -->
  <!-- ---------------------------------------------- -->
  <v-list-group no-action>
    <!-- ---------------------------------------------- -->
    <!---Dropdown  -->
    <!-- ---------------------------------------------- -->
    <template v-slot:activator="{ props }">
      <v-list-item active-class="test" :exact="true" v-bind="props" :value="item.title" rounded class="mb-1" color="secondary">
        <!---Icon  -->
        <template v-slot:prepend>
          <icon-set :item="item.icon" :level="level" />
        </template>
        <!---Title  -->
        <v-list-item-title class="mr-auto">{{ t(item.title) }}</v-list-item-title>
        <!---If Caption-->
        <v-list-item-subtitle v-if="item.subCaption" class="text-caption mt-n1 hide-menu">
          {{ t(item.subCaption) }}
        </v-list-item-subtitle>
      </v-list-item>
    </template>
    <!-- ---------------------------------------------- -->
    <!---Sub Item-->
    <!-- ---------------------------------------------- -->
    <template v-for="(subitem, i) in item.children" :key="i">
      <nav-collapse :item="subitem" v-if="subitem.children" :level="props.level + 1" />
      <nav-item :item="subitem" :level="props.level + 1" v-else />
    </template>
  </v-list-group>

  <!-- ---------------------------------------------- -->
  <!---End Item Sub Header -->
  <!-- ---------------------------------------------- -->
</template>
