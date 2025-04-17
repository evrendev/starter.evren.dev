<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useUserStore } from "@/stores/";
import { ParentCard } from "@/components/shared/";
import { ConfirmDialog } from "@/components/forms/";
import config from "@/config";

const { t } = useI18n();
const userStore = useUserStore();

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

defineEmits(["update:options"]);

const headers = ref([
  { title: t("admin.users.fields.initial"), key: "initial", sortable: false },
  { title: t("admin.users.fields.twoFactorEnabled"), key: "twoFactorEnabled", sortable: false },
  { title: t("admin.users.fields.gender"), key: "gender", sortable: false },
  { title: t("admin.users.fields.firstName"), key: "firstName", sortable: true },
  { title: t("admin.users.fields.lastName"), key: "lastName", sortable: true },
  { title: t("admin.users.fields.email"), key: "email", sortable: true },
  { title: t("admin.users.fields.process"), key: "process", align: "center", sortable: false, width: "64px" }
]);

const dataTableRef = ref(null);
const operation = ref("delete");
const showModal = ref(false);
const userId = ref(null);
const confirmModalTitle = ref(null);
const confirmModalMessage = ref(null);

const handleConfirm = async () => {
  if (operation.value === "delete") {
    await userStore.delete(userId.value);
  } else {
    await userStore.restore(userId.value);
  }
};

const handleCancel = () => {
  userId.value = null;
};

const showConfirmDialog = (id, opt) => {
  if (opt === "delete") {
    confirmModalTitle.value = t("admin.users.delete.title");
    confirmModalMessage.value = t("admin.users.delete.message");
  } else {
    confirmModalTitle.value = t("admin.users.restore.title");
    confirmModalMessage.value = t("admin.users.restore.message");
  }

  operation.value = opt;
  userId.value = id;
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
      <template #[`item.initial`]="{ item }">
        <v-avatar size="24" :color="item.deleted ? 'error' : 'primary'">
          <span class="text-h6 text-white">
            {{ item?.initial }}
          </span>
        </v-avatar>
      </template>
      <template #[`item.email`]="{ item }">
        <a :href="`mailto:${item.email}`">{{ item.email }}</a>
      </template>
      <template #[`item.twoFactorEnabled`]="{ item }">
        <v-icon
          size="small"
          :icon="item.twoFactorEnabled ? `$shieldAccount` : `$shieldLockOpen`"
          :color="item.twoFactorEnabled ? `success` : `error`"
        />
      </template>
      <template #[`item.process`]="{ item }">
        <router-link :to="{ name: 'edit-user', params: { id: item.id } }">
          <v-icon size="small" icon="$pencil" color="secondary" />
        </router-link>
        <v-icon
          size="small"
          icon="$restore"
          color="success"
          class="ml-2"
          v-show="hasUserRestorePermission && item.deleted"
          @click="showConfirmDialog(item.id, `restore`)"
          :title="t('admin.users.restore.title')"
        />
        <v-icon
          size="small"
          icon="$trashCan"
          color="error"
          class="ml-2"
          v-show="hasUserDeletePermission && !item.deleted"
          @click="showConfirmDialog(item.id, `delete`)"
          :title="t('admin.users.delete.title')"
        />
      </template>
    </v-data-table-server>
  </parent-card>

  <confirm-dialog
    v-model="showModal"
    :title="confirmModalTitle"
    :message="confirmModalMessage"
    @confirm="handleConfirm"
    @cancel="handleCancel"
  />
</template>
