<script lang="ts" setup>
import { Chapter } from "@/models/chapter";
import { AdvancedFilters } from "@/types/requests/lesson";

const { t } = useI18n();

defineProps<{
  pageTitle: string;
  disabled: boolean;
  chapters: Chapter[];
}>();

const filters = ref<AdvancedFilters>({
  search: null,
  chapterId: null,
});

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
    chapterId: null,
  };

  emit("reset");
};
</script>

<template>
  <v-card elevation="6" class="mt-4" :title="pageTitle">
    <v-card-text>
      <v-form :disabled="disabled">
        <v-row>
          <v-col cols="12" md="3">
            <v-select
              v-model="filters.chapterId"
              :items="chapters"
              hide-details
              item-value="id"
              item-title="title"
              variant="outlined"
              :label="t('admin.lessons.fields.chapterId.title')"
            />
          </v-col>
          <v-col cols="12" md="7">
            <v-text-field
              v-model="filters.search"
              variant="outlined"
              :label="t('shared.filters.search')"
            />
          </v-col>
          <v-col cols="12" md="2" class="d-flex justify-end align-center gap-2">
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
