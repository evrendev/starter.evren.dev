<script lang="ts" setup>
import { AdvancedFilters } from "@/types/requests/personal";

const { t } = useI18n();

defineProps<{
  pageTitle: string;
  disabled: boolean;
}>();

const filters = ref<AdvancedFilters>({
  search: null,
  startDate: null,
  endDate: null,
});

const items = ref([
  { value: null, text: t("shared.options.isActive.all") },
  { value: true, text: t("shared.options.isActive.true") },
  { value: false, text: t("shared.options.isActive.false") },
]);

const emit = defineEmits<{
  (e: "submit", values: AdvancedFilters): void;
  (e: "reset"): void;
}>();

const submit = () => {
  emit("submit", filters.value);
};

const reset = () => {
  filters.value = {
    search: null,
    startDate: null,
    endDate: null,
  };

  emit("reset");
};
</script>

<template>
  <v-card elevation="6" class="mt-4" :title="pageTitle">
    <v-card-text>
      <v-form :disabled="disabled">
        <v-row>
          <v-col cols="12" md="6">
            <v-text-field
              v-model="filters.search"
              variant="outlined"
              :label="t('shared.filters.search')"
            />
          </v-col>
          <v-col cols="12" md="3">
            <v-text-field
              v-model="filters.startDate"
              type="date"
              variant="outlined"
              hide-details
              :label="t('shared.filters.startDate')"
            />
          </v-col>
          <v-col cols="12" md="3">
            <v-text-field
              v-model="filters.endDate"
              type="date"
              variant="outlined"
              hide-details
              :label="t('shared.filters.endDate')"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" class="d-flex justify-end gap-2">
            <v-btn
              color="secondary"
              size="small"
              prepend-icon="bx-reset"
              @click="reset"
            >
              {{ t("shared.reset") }}
            </v-btn>
            <v-btn
              color="primary"
              size="small"
              prepend-icon="bx-filter"
              @click="submit"
            >
              {{ t("shared.filter") }}
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>
</template>
