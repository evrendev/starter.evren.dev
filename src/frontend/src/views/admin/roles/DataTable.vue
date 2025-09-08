<script setup lang="ts">
import StatusIcon from "@/components/admin/StatusIcon.vue";
import { Role } from "@/requests/role";
import { useDateFormat } from "@vueuse/core";
const { t, locale } = useI18n();

defineProps<{
  itemsPerPage: number;
  itemValue: string;
  items: Array<any>;
  total: number;
  loading: boolean;
  headers: Array<any>;
}>();

const emit = defineEmits<{
  (e: "delete", id: string | null): void;
  (e: "update:options", options: any): void;
}>();

const roleId: Ref<string> = ref("");
const toggleDeleteConfirmDialog = ref(false);
const dialogTitle: Ref<string | null> = ref(null);
const dialogMessage: Ref<string | null> = ref(null);

const showDeleteConfirmModal = (role: Role) => {
  roleId.value = role.id;
  dialogTitle.value = t("admin.roles.notifications.deleteConfirm", {
    name: role.name,
  });
  dialogMessage.value = t("admin.roles.notifications.deleteMessage", {
    name: role.name,
  });
  toggleDeleteConfirmDialog.value = true;
};

const confirmDelete = () => {
  emit("delete", roleId.value);
};

const abortDelete = () => {
  roleId.value = "";
  toggleDeleteConfirmDialog.value = false;
};
</script>

<template>
  <v-card elevation="6" class="mt-4">
    <v-card-title>
      <toolbar
        :title="t('admin.roles.list.title')"
        :button="{
          icon: 'bx-plus',
          text: t('shared.new'),
          to: { name: 'role-create' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-data-table-server
        :items-per-page="itemsPerPage"
        :items="items"
        :items-length="total"
        :item-value="itemValue"
        :headers="headers"
        :loading="loading"
        @update:options="emit('update:options', $event)"
        class="striped border"
      >
        <template #[`item.id`]="{ item }">
          <router-link
            :to="{ name: 'role-view', params: { id: item.id } }"
            :text="item.id"
          />
        </template>

        <template #[`item.isActive`]="{ item }">
          <status-icon :isActive="item.isActive" />
        </template>

        <template #[`item.validUpto`]="{ item }">
          {{
            useDateFormat(item.validUpto, "DD MMM YYYY", {
              locales: locale,
            })
          }}
        </template>

        <template #[`item.actions`]="{ item }">
          <v-menu>
            <template v-slot:activator="{ props }">
              <v-btn
                color="primary"
                v-bind="props"
                size="small"
                append-icon="bx-chevron-down"
              >
                {{ t("shared.actions") }}
              </v-btn>
            </template>
            <v-list :lines="false" density="compact" nav>
              <v-list-item :to="{ name: 'role-view', params: { id: item.id } }">
                <v-list-item-title v-text="t('shared.view')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-show" />
                </template>
              </v-list-item>

              <v-list-item :to="{ name: 'role-edit', params: { id: item.id } }">
                <v-list-item-title v-text="t('shared.edit')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-edit" />
                </template>
              </v-list-item>

              <v-list-item @click="showDeleteConfirmModal(item)">
                <v-list-item-title v-text="t('shared.delete')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-trash" />
                </template>
              </v-list-item>
            </v-list>
          </v-menu>
        </template>
      </v-data-table-server>
    </v-card-text>
  </v-card>

  <confirm-dialog
    v-model:show-dialog="toggleDeleteConfirmDialog"
    :confirm-button-text="t('shared.confirm')"
    :cancel-button-text="t('shared.cancel')"
    :title="dialogTitle"
    :message="dialogMessage"
    @confirm="confirmDelete"
    @cancel="abortDelete"
  />
</template>
