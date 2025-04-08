<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { ParentCard } from "@/components/shared/";
import { ConfirmModal } from "@/components/forms/";
import { useRoleStore } from "@/stores/";
import config from "@/config";

const { t } = useI18n();
const roleStore = useRoleStore();

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
  { title: t("admin.roles.fields.name"), key: "name", sortable: false },
  { title: t("admin.roles.fields.description"), key: "description", sortable: false },
  { title: t("admin.roles.fields.process"), key: "process", align: "center", sortable: false, width: "64px" }
]);

const dataTableRef = ref(null);
const operation = ref("delete");
const showModal = ref(false);
const roleId = ref(null);
const confirmModalTitle = ref(null);
const confirmModalMessage = ref(null);

const handleConfirm = async () => {
  if (operation.value === "delete") {
    await roleStore.delete(roleId.value);
  } else {
    await roleStore.activate(roleId.value);
  }
};

const handleCancel = () => {
  roleId.value = null;
};

const showConfirmModal = (id) => {
  confirmModalTitle.value = t("admin.roles.delete.title");
  confirmModalMessage.value = t("admin.roles.delete.message");

  roleId.value = id;
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
      density="compact"
      class="striped"
      item-value="id"
      @update:options="$emit('update:options', $event)"
    >
      <template #[`item.isActive`]="{ item }">
        <v-icon size="small" :icon="item.isActive ? `$thumbUp` : `$thumbDown`" :color="item.isActive ? `success` : `error`" />
      </template>

      <template #[`item.process`]="{ item }">
        <router-link :to="{ name: 'edit-role', params: { id: item.id } }">
          <v-icon size="small" icon="$pencil" color="secondary" />
        </router-link>
        <v-icon
          size="small"
          icon="$trashCan"
          color="error"
          class="ml-2"
          @click="showConfirmModal(item.id, `delete`, true)"
          :title="t('admin.roles.delete.title')"
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
