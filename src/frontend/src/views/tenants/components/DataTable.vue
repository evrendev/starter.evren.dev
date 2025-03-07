<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { ParentCard } from "@/components/shared/";
import { ConfirmModal } from "@/components/forms/";
import { useTenantStore } from "@/stores/";
import config from "@/config";

const { t } = useI18n();
const tenantStore = useTenantStore();

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
  { title: t("admin.tenants.fields.name"), key: "name", sortable: true },
  { title: t("admin.tenants.fields.adminEmail"), key: "adminEmail", sortable: false },
  { title: t("admin.tenants.fields.validUntil"), key: "validUntil", align: "center", sortable: true },
  { title: t("admin.tenants.fields.process"), key: "process", align: "center", sortable: false, width: "64px" }
]);

const dataTableRef = ref(null);
const operation = ref("delete");
const recover = ref(false);
const showModal = ref(false);
const tenantId = ref(null);
const confirmModalTitle = ref(null);
const confirmModalMessage = ref(null);

const handleConfirm = async () => {
  if (operation.value === "delete" && recover.value) {
    await tenantStore.delete(tenantId.value);
  } else if (operation.value === "delete" && !recover.value) {
    await tenantStore.restore(tenantId.value);
  } else if (operation.value === "activate" && recover.value) {
    await tenantStore.activate(tenantId.value);
  } else {
    await tenantStore.deactivate(tenantId.value);
  }
};

const handleCancel = () => {
  tenantId.value = null;
};

const showConfirmModal = (id, opt, rec) => {
  let modalTitle = null;
  let modalMessage = null;

  if (opt === "activate") {
    modalTitle = rec ? t("admin.tenants.deactivate.title") : t("admin.tenants.activate.title");
    modalMessage = rec ? t("admin.tenants.deactivate.message") : t("admin.tenants.activate.message");
  } else {
    modalTitle = rec ? t("admin.tenants.delete.title") : t("admin.tenants.restore.title");
    modalMessage = rec ? t("admin.tenants.delete.message") : t("admin.tenants.restore.message");
  }

  confirmModalTitle.value = modalTitle;
  confirmModalMessage.value = modalMessage;

  operation.value = opt;
  recover.value = rec;
  tenantId.value = id;
  showModal.value = true;
};
</script>

<template>
  <parent-card>
    <v-data-table-server
      ref="dataTableRef"
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
          @click="showConfirmModal(item.id, 'delete', false)"
          :title="t('admin.tenants.restore.title')"
        />
        <v-icon
          v-else
          size="small"
          icon="$trashCan"
          color="error"
          class="ml-2"
          @click="showConfirmModal(item.id, 'delete', true)"
          :title="t('admin.tenants.delete.title')"
        />
        <v-icon
          v-if="item.isActive"
          icon="$thumbDown"
          size-="small"
          color="warning"
          class="ml-2"
          @click="showConfirmModal(item.id, 'activate', true)"
          :title="t('admin.tenants.deactivate.title')"
        />
        <v-icon
          v-else
          size="small"
          icon="$thumbUp"
          size-="small"
          color="success"
          class="ml-2"
          @click="showConfirmModal(item.id, 'activate', false)"
          :title="t('admin.tenants.activate.title')"
        />
      </template>
    </v-data-table-server>
  </parent-card>

  <confirm-modal
    v-model="showModal"
    :title="confirmModalTitle"
    :message="confirmModalMessage"
    @confirm="handleConfirm"
    @cancel="handleCancel"
  />
</template>
