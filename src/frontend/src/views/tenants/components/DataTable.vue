<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { ParentCard } from "@/components/shared/";
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
  { title: t("admin.tenants.fields.isActive"), key: "isActive", align: "center", sortable: false, width: "128px" },
  { title: t("admin.tenants.fields.name"), key: "name", sortable: false },
  { title: t("admin.tenants.fields.admin"), key: "adminEmail", sortable: false },
  { title: t("admin.tenants.fields.validUntil"), key: "validUntil.displayDateWithTime", align: "center", sortable: true },
  { title: t("admin.tenants.fields.process"), key: "process", align: "center", sortable: false, width: "64px" }
]);

const promptDelete = (id) => {
  console.log(id);
};
</script>

<template>
  <parent-card>
    <v-data-table-server
      :items-per-page="config.itemsPerPage"
      :headers="headers"
      :items="items"
      :items-length="itemsLength"
      :loading="loading"
      class="striped"
      item-value="id"
      @update:options="$emit('update:options', $event)"
    >
      <template #[`item.isActive`]="{ item }">
        <v-icon size="small" :icon="item.isActive ? `$thumbUp` : `$thumbDown`" :color="item.isActive ? `success` : `error`" />
      </template>

      <template #[`item.process`]="{ item }">
        <router-link :to="{ name: 'edit-tenant', params: { id: item.id } }">
          <v-icon size="small" icon="$pencil" />
        </router-link>
        <v-icon size="small" icon="$trashCan" size-="small" color="error" class="ml-2" @click.once="promptDelete(item.id)" />
      </template>
    </v-data-table-server>
  </parent-card>
</template>
