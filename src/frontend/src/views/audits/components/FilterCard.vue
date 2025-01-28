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
const selectedAction = ref(null);
const actions = ref([
  { title: t("common.all"), value: null },
  { title: t("admin.audits.actions.insert"), value: "Insert" },
  { title: t("admin.audits.actions.update"), value: "Update" },
  { title: t("admin.audits.actions.delete"), value: "Delete" },
  { title: t("admin.audits.actions.recovered"), value: "Recovered" }
]);

const handleSubmit = () => {
  emits("submit", {
    search: search.value,
    action: selectedAction.value,
    startDate: dateRange.value[0],
    endDate: dateRange.value[1]
  });
};

const handleReset = () => {
  search.value = "";
  selectedAction.value = null;
  dateRange.value = [null, null];
  emits("reset");
};
</script>

<template>
  <parent-card class="mb-4" :title="t('common.filters')">
    <v-row>
      <v-col cols="12" md="3">
        <v-text-field v-model="search" :label="t('common.search')" density="comfortable" hide-details variant="outlined"></v-text-field>
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="selectedAction"
          :items="actions"
          :label="t('admin.audits.fields.action')"
          density="comfortable"
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
              density="comfortable"
              hide-details
              type="date"
              variant="outlined"
            ></v-text-field>
          </v-col>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="dateRange[1]"
              :label="t('common.selectDate')"
              density="comfortable"
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
