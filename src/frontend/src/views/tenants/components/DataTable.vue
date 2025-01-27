<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import UiParentCard from "@/components/shared/UiParentCard.vue";
import config from "@/config";

const { t } = useI18n();

defineProps({
  items: {
    type: Array,
    required: true
  },
  itemsLength: {
    type: Number,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
});

defineEmits(["update:options"]);

const headers = ref([
  { title: t("admin.tenants.fields.name"), key: "name", sortable: false },
  { title: t("admin.tenants.fields.admin"), key: "adminEmail", sortable: false },
  { title: t("admin.tenants.fields.validUntil"), key: "validUntil.displayDateWithTime", align: "center", sortable: true },
  { title: t("admin.tenants.fields.isActive"), key: "isActive", align: "center", sortable: false }
]);
</script>

<template>
  <UiParentCard>
    <v-data-table-server
      :items-per-page="config.itemsPerPage"
      :headers="headers"
      :items="items"
      :items-length="itemsLength"
      :loading="loading"
      class="striped"
      item-value="id"
      @update:options="$emit('update:options', $event)"
    />
  </UiParentCard>
</template>
