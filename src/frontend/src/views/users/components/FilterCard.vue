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
  hasUserDeletePermission: {
    type: Boolean,
    default: false
  },
  hasUserRestorePermission: {
    type: Boolean,
    default: false
  }
});

const emits = defineEmits(["submit", "reset"]);

const search = ref("");
const dateRange = ref([null, null]);
const showDeletedItems = ref(false);
const showDeletedItemsOptions = ref([
  { title: t("admin.users.delete.options.true"), value: true },
  { title: t("admin.users.delete.options.false"), value: false }
]);

const handleSubmit = () => {
  emits("submit", {
    search: search.value,
    startDate: dateRange.value[0],
    endDate: dateRange.value[1],
    showDeletedItems: showDeletedItems.value
  });
};

const handleReset = () => {
  search.value = "";
  dateRange.value = [null, null];
  showDeletedItems.value = false;

  emits("reset");
};
</script>

<template>
  <parent-card class="mb-4" :title="t('common.filters')">
    <v-row>
      <v-col cols="12" :md="hasUserDeletePermission ? 4 : 6">
        <v-text-field
          v-model="search"
          :disabled="loading"
          :label="t('common.search')"
          density="comfortable"
          hide-details
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="2" v-show="hasUserDeletePermission">
        <v-select
          v-model="showDeletedItems"
          :items="showDeletedItemsOptions"
          :label="t('common.showDeletedItems')"
          :disabled="loading"
          density="comfortable"
          hide-details
          item-title="title"
          item-value="value"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-text-field
          v-model="dateRange[0]"
          :label="t('common.selectDate')"
          density="comfortable"
          :disabled="loading"
          hide-details
          type="date"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-text-field
          v-model="dateRange[1]"
          :label="t('common.selectDate')"
          density="comfortable"
          :disabled="loading"
          hide-details
          type="date"
          variant="outlined"
        />
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
