<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { ParentCard } from "@/components/shared/";

const { t } = useI18n();

defineProps({
  loading: {
    type: Boolean,
    default: false
  },
  hasTenantDeletePermission: {
    type: Boolean,
    default: false
  },
  hasTenantRestorePermission: {
    type: Boolean,
    default: false
  }
});

const emits = defineEmits(["submit", "reset"]);

const search = ref("");
const dateRange = ref([null, null]);
const isActive = ref(null);
const activeOptions = ref([
  { title: t("common.all"), value: null },
  { title: t("common.true"), value: true },
  { title: t("common.false"), value: false }
]);
const showDeletedItems = ref(false);
const showDeletedItemsOptions = ref([
  { title: t("common.true"), value: true },
  { title: t("common.false"), value: false }
]);

const handleSubmit = () => {
  emits("submit", {
    search: search.value,
    isActive: isActive.value,
    showDeletedItems: showDeletedItems.value,
    startDate: dateRange.value[0],
    endDate: dateRange.value[1]
  });
};

const handleReset = () => {
  search.value = "";
  isActive.value = null;
  showDeletedItems.value = false;
  dateRange.value = [null, null];
  emits("reset");
};
</script>

<template>
  <parent-card class="mb-4" :title="t('common.filters')">
    <v-row>
      <v-col cols="12" md="4">
        <v-text-field v-model="search" :label="t('common.search')" density="comfortable" hide-details variant="outlined"></v-text-field>
      </v-col>
      <v-col cols="12" md="2">
        <v-select
          v-model="isActive"
          :items="activeOptions"
          :label="t('common.showOnlyActiveItems')"
          density="comfortable"
          hide-details
          item-title="title"
          item-value="value"
          variant="outlined"
        ></v-select>
      </v-col>
      <v-col cols="12" md="2">
        <v-select
          v-model="showDeletedItems"
          :items="showDeletedItemsOptions"
          :label="t('common.showDeletedItems')"
          density="comfortable"
          hide-details
          item-title="title"
          item-value="value"
          variant="outlined"
        ></v-select>
      </v-col>
      <v-col cols="12" md="4">
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
