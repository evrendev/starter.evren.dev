<script setup>
import { ref, nextTick } from "vue";
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

const emit = defineEmits(["update:options"]);

const headers = ref([
  { title: t("admin.tenants.fields.isActive"), key: "isActive", align: "center", sortable: false, width: "128px" },
  { title: t("admin.tenants.fields.name"), key: "name", sortable: false },
  { title: t("admin.tenants.fields.admin"), key: "adminEmail", sortable: false },
  { title: t("admin.tenants.fields.validUntil"), key: "validUntil.displayDateWithTime", align: "center", sortable: true },
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
  } else {
    await tenantStore.activate(tenantId.value);
  }

  await nextTick();

  emit("update:options", {
    page: 1,
    itemsPerPage: config.itemsPerPage,
    sortBy: null,
    groupBy: null,
    isActive: null,
    startDate: null,
    endDate: null,
    search: null
  });
};

const handleCancel = () => {
  tenantId.value = null;
};

const showConfirmModal = (id, opt, value) => {
  let activateTitle = null;
  let activateMessage = null;

  if (opt === "activate") {
    activateTitle = value ? t("admin.tenants.deactivate.title") : t("admin.tenants.activate.title");
    activateMessage = value ? t("admin.tenants.deactivate.message") : t("admin.tenants.activate.message");
  } else {
    activateTitle = t("admin.tenants.delete.title");
    activateMessage = t("admin.tenants.delete.message");
  }

  confirmModalTitle.value = activateTitle;
  confirmModalMessage.value = activateMessage;

  operation.value = opt;
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

      <template #[`item.process`]="{ item }">
        <router-link :to="{ name: 'edit-tenant', params: { id: item.id } }">
          <v-icon size="small" icon="$pencil" color="secondary" />
        </router-link>
        <v-icon
          v-show="item.isActive"
          size="small"
          icon="$trashCan"
          color="error"
          class="ml-2"
          @click="showConfirmModal(item.id, `delete`, true)"
          :title="t('admin.tenants.delete.title')"
        />
        <v-icon
          v-if="item.isActive"
          icon="$thumbDown"
          size-="small"
          color="warning"
          class="ml-2"
          @click="showConfirmModal(item.id, `activate`, true)"
          :title="t('admin.tenants.deactivate.title')"
        />
        <v-icon
          v-else
          size="small"
          icon="$thumbUp"
          size-="small"
          color="success"
          class="ml-2"
          @click="showConfirmModal(item.id, `activate`, false)"
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
