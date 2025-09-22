<script lang="ts" setup>
import { Category } from "@/models/category";
import { BasicFilters } from "@/types/requests/course";

const { t } = useI18n();

defineProps<{
  pageTitle: string;
  disabled: boolean;
  categories: Category[];
}>();

const filters = ref<BasicFilters>({
  search: null,
  categoryId: null,
  published: null,
});

const publishedItems = ref([
  { value: null, text: t("shared.options.published.all") },
  { value: true, text: t("shared.options.published.true") },
  { value: false, text: t("shared.options.published.false") },
]);

const emit = defineEmits<{
  (e: "submit", values: BasicFilters): void;
  (e: "reset"): void;
}>();

const submit = () => {
  emit("submit", filters.value);
};

const reset = () => {
  filters.value = {
    search: null,
    categoryId: null,
    published: null,
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
              v-model="filters.published"
              :items="publishedItems"
              hide-details
              item-text="value"
              item-title="text"
              variant="outlined"
              :label="t('shared.filters.published')"
            />
          </v-col>
          <v-col cols="12" md="3">
            <v-select
              v-model="filters.categoryId"
              :items="categories"
              hide-details
              item-value="id"
              item-title="name"
              variant="outlined"
              :label="t('admin.courses.fields.categoryId.title')"
            />
          </v-col>
          <v-col cols="12" md="4">
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
