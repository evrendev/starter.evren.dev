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

const emits = defineEmits(["submit", "reset", "getLastDonations"]);

const search = ref("");
const dateRange = ref([null, null]);

const handleSubmit = () => {
  emits("submit", {
    search: search.value,
    startDate: dateRange.value[0],
    endDate: dateRange.value[1]
  });
};

const handleReset = () => {
  search.value = "";
  dateRange.value = [null, null];
  emits("reset");
};

const getLastDonations = () => {
  emits("getLastDonations");
};
</script>

<template>
  <parent-card
    class="mb-4"
    :title="t('common.filters')"
    :actionButton="{
      text: t('common.getLastDonations'),
      color: 'warning',
      icon: '$download',
      loading: loading,
      onClick: getLastDonations,
      size: 'x-small',
      disabled: loading
    }"
  >
    <v-row>
      <v-col cols="12" md="6">
        <v-text-field v-model="search" :label="t('common.search')" density="compact" hide-details variant="outlined"></v-text-field>
      </v-col>
      <v-col cols="12" md="6">
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="dateRange[0]"
              :label="t('common.startDate')"
              density="compact"
              hide-details
              type="date"
              variant="outlined"
            ></v-text-field>
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="dateRange[1]"
              :label="t('common.endDate')"
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
