<script setup>
import { useI18n } from "vue-i18n";
import { IconSet } from "./";

const { t } = useI18n();

const props = defineProps({ item: Object, level: Number });
</script>

<template>
  <!---Single Item-->
  <v-list-item
    :to="item.type === 'external' ? '' : item.to"
    :href="item.type === 'external' ? item.to : ''"
    :disabled="item.disabled"
    :target="item.type === 'external' ? '_blank' : ''"
    :exact="true"
    class="mb-1"
    color="secondary"
    rounded
  >
    <!---If icon-->
    <template v-slot:prepend>
      <icon-set :item="props.item.icon" :level="props.level" />
    </template>
    <v-list-item-title>{{ t(item.title) }}</v-list-item-title>
    <!---If Caption-->
    <v-list-item-subtitle v-if="item.subCaption" class="text-caption mt-n1 hide-menu">
      {{ t(item.subCaption) }}
    </v-list-item-subtitle>
    <!---If any chip or label-->
    <template v-slot:append v-if="item.chip">
      <v-chip
        :color="item.chipColor"
        class="sidebarchip hide-menu"
        :size="item.chipIcon ? 'small' : 'default'"
        :variant="item.chipVariant"
        :prepend-icon="item.chipIcon"
      >
        {{ item.chip }}
      </v-chip>
    </template>
  </v-list-item>
</template>
