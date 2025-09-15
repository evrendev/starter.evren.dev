<script setup lang="ts">
import StatusIcon from "@/components/admin/StatusIcon.vue";
import { User } from "@/models/user";
import { Props } from "@/types/requests/app";
const { t } = useI18n();

withDefaults(defineProps<Props<User>>(), {
  itemsPerPage: 25,
  items: () => [],
  total: 0,
  loading: false,
  headers: () => [],
});

const emit = defineEmits<{
  (e: "delete", id: string | null): void;
  (e: "update:options", options: any): void;
}>();

const userId: Ref<string> = ref("");
const toggleDeleteConfirmDialog = ref(false);
const dialogTitle: Ref<string | null> = ref(null);
const dialogMessage: Ref<string | null> = ref(null);

const showDeleteConfirmModal = (user: User) => {
  userId.value = user.id as string;
  dialogTitle.value = t("admin.users.notifications.deleteConfirm", {
    name: user.fullName,
  });
  dialogMessage.value = t("admin.users.notifications.deleteMessage", {
    name: user.fullName,
  });
  toggleDeleteConfirmDialog.value = true;
};

const confirmDelete = () => {
  emit("delete", userId.value);
};

const abortDelete = () => {
  userId.value = "";
  toggleDeleteConfirmDialog.value = false;
};
</script>

<template>
  <v-card elevation="6" class="mt-4">
    <v-card-title>
      <toolbar
        :title="t('admin.users.list.title')"
        :button="{
          icon: 'bx-plus',
          text: t('shared.new'),
          to: { name: 'user-create' },
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
        <template #[`item.initial`]="{ item }">
          <v-avatar color="primary" variant="tonal">
            {{ item.initial }}
          </v-avatar>
        </template>

        <template #[`item.isActive`]="{ item }">
          <status-icon :isActive="item.isActive" />
        </template>

        <template #[`item.twoFactorEnabled`]="{ item }">
          <status-icon :isActive="item.twoFactorEnabled" />
        </template>

        <template #[`item.fullName`]="{ item }">
          <router-link
            :to="{ name: 'user-view', params: { id: item.id } }"
            :text="item.fullName"
          />
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
              <v-list-item :to="{ name: 'user-view', params: { id: item.id } }">
                <v-list-item-title v-text="t('shared.view')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-show" />
                </template>
              </v-list-item>

              <v-list-item :to="{ name: 'user-edit', params: { id: item.id } }">
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
