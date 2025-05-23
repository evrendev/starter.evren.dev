<script setup>
import ReportTable from "./ReportTable.vue";

defineProps({
  projects: {
    type: Array,
    default: () => []
  },
  isoYear: {
    type: Number,
    default: new Date().getFullYear(),
    required: false
  },
  isoWeekNumber: {
    type: Number,
    default: 1,
    required: false
  }
});
</script>
<template>
  <v-row>
    <v-col md="12">
      <v-sheet class="pa-2">
        <h2 class="text-center mt-2">{{ isoYear }}-KW{{ isoWeekNumber }}</h2>
        <report-table v-for="item in projects" :key="item.project.name" :item="item" />
      </v-sheet>
    </v-col>
  </v-row>
</template>

<style lang="scss" scoped></style>
