<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { ParentCard } from "@/components/shared/";
import { ConfirmDialog } from "@/components/forms/";
import { useTenantStore } from "@/stores/";

const { t } = useI18n();
const tenantStore = useTenantStore();

defineProps({
  items: {
    type: Array,
    required: true
  },
  itemsPerPage: {
    type: Number,
    required: true
  },
  page: {
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
  { title: t("admin.tenants.fields.name"), key: "name", sortable: true },
  { title: t("admin.tenants.fields.adminEmail"), key: "adminEmail", sortable: false },
  { title: t("admin.tenants.fields.validUntil"), key: "validUntil", align: "center", sortable: true },
  { title: t("admin.tenants.fields.process"), key: "process", align: "center", sortable: false, width: "64px" }
]);

const dataTableRef = ref(null);
const operation = ref("delete");
const showModal = ref(false);
const tenantId = ref(null);
const confirmModalTitle = ref(null);
const confirmModalMessage = ref(null);

const handleConfirm = async () => {
  if (operation.value === "delete") {
    await tenantStore.delete(tenantId.value);
  } else if (operation.value === "restore") {
    await tenantStore.restore(tenantId.value);
  } else if (operation.value === "activate") {
    await tenantStore.activate(tenantId.value);
  } else if (operation.value === "deactivate") {
    await tenantStore.deactivate(tenantId.value);
  }
};

const handleCancel = () => {
  tenantId.value = null;
};

const showConfirmDialog = (id, opt) => {
  confirmModalTitle.value = t(`admin.tenants.${opt}.title`);
  confirmModalMessage.value = t(`admin.tenants.${opt}.message`);

  operation.value = opt;
  tenantId.value = id;
  showModal.value = true;
};

const updatePage = (page) => {
  tenantStore.setFilters({ page });
};

const updateItemsPerPage = (itemsPerPage) => {
  tenantStore.setFilters({ itemsPerPage });
};
</script>

<template>
  <parent-card>
    <v-data-table-server
      ref="dataTableRef"
      :items-per-page="itemsPerPage"
      :headers="headers"
      :items="items"
      :items-length="itemsPerPage"
      :page="page"
      :loading="loading"
      density="compact"
      class="striped"
      item-value="id"
      @update:options="$emit('update:options', $event)"
      @update:page="updatePage"
      @update:items-per-page="updateItemsPerPage"
    >
      <template #[`item.isActive`]="{ item }">
        <v-icon size="small" :icon="item.isActive ? `$thumbUp` : `$thumbDown`" :color="item.isActive ? `success` : `error`" />
      </template>

      <template #[`item.validUntil`]="{ item }">
        {{ item.validUntil.displayDate }}
      </template>

      <template #[`item.process`]="{ item }">
        <router-link :to="{ name: 'edit-tenant', params: { id: item.id } }">
          <v-icon size="small" icon="$pencil" color="secondary" />
        </router-link>
        <v-icon
          v-if="item.deleted"
          size="small"
          icon="$restore"
          color="error"
          class="ml-2"
          @click="showConfirmDialog(item.id, 'restore')"
          :title="t('admin.tenants.restore.title')"
        />
        <v-icon
          v-else
          size="small"
          icon="$trashCan"
          color="error"
          class="ml-2"
          @click="showConfirmDialog(item.id, 'delete')"
          :title="t('admin.tenants.delete.title')"
        />
        <v-icon
          v-if="item.isActive"
          icon="$thumbDown"
          size-="small"
          color="warning"
          class="ml-2"
          @click="showConfirmDialog(item.id, 'deactivate')"
          :title="t('admin.tenants.deactivate.title')"
        />
        <v-icon
          v-else
          size="small"
          icon="$thumbUp"
          size-="small"
          color="success"
          class="ml-2"
          @click="showConfirmDialog(item.id, 'activate')"
          :title="t('admin.tenants.activate.title')"
        />
      </template>
    </v-data-table-server>
  </parent-card>

  <confirm-dialog
    v-model="showModal"
    :title="confirmModalTitle"
    :message="confirmModalMessage"
    :confirm-button-text="t('common.confirm')"
    :cancel-button-text="t('common.cancel')"
    @confirm="handleConfirm"
    @cancel="handleCancel"
  />
</template>
