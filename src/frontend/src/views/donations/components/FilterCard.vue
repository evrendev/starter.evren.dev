<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { ParentCard } from "@/components/shared/";

const { t } = useI18n();

defineProps({
  loading: {
    type: Boolean,
    default: false
  }
});

const emits = defineEmits(["submit", "reset"]);

const search = ref("");
const dateRange = ref([null, null]);
const selectedProjectCode = ref(null);
const projectCodes = ref([
  { title: t("common.all"), value: null },
  { title: t("admin.donations.projectCodes.bks"), value: "BKS" },
  { title: t("admin.donations.projectCodes.bgs"), value: "BGS" },
  { title: t("admin.donations.projectCodes.aki"), value: "AKI" },
  { title: t("admin.donations.projectCodes.agi"), value: "AGI" }
]);

const handleSubmit = () => {
  emits("submit", {
    search: search.value,
    projectCode: selectedProjectCode.value,
    startDate: dateRange.value[0],
    endDate: dateRange.value[1]
  });
};

const handleReset = () => {
  search.value = "";
  selectedProjectCode.value = null;
  dateRange.value = [null, null];
  emits("reset");
};
</script>

<template>
  <parent-card class="mb-4" :title="t('common.filters')">
    <v-row>
      <v-col cols="12" md="3">
        <v-text-field v-model="search" :label="t('common.search')" density="compact" hide-details variant="outlined"></v-text-field>
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="selectedProjectCode"
          :items="projectCodes"
          :label="t('admin.donations.fields.projectCode')"
          density="compact"
          hide-details
          item-title="title"
          item-value="value"
          variant="outlined"
        ></v-select>
      </v-col>
      <v-col cols="12" md="6">
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="dateRange[0]"
              :label="t('common.selectDate')"
              density="compact"
              hide-details
              type="date"
              variant="outlined"
            ></v-text-field>
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="dateRange[1]"
              :label="t('common.selectDate')"
              density="compact"
              hide-details
              type="date"
              variant="outlined"
            ></v-text-field>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
    <v-row class="mt-2">
      <v-col cols="12" class="d-flex justify-end ga-2">
        <v-btn color="error" :disabled="loading" @click="handleReset" prepend-icon="$refresh">
          {{ t("common.reset") }}
        </v-btn>
        <v-btn color="primary" :loading="loading" @click="handleSubmit" prepend-icon="$filterCheck">
          {{ t("common.submit") }}
        </v-btn>
      </v-col>
    </v-row>
  </parent-card>
</template>
