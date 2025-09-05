<script setup lang="ts">
import StatusIcon from "@/components/admin/StatusIcon.vue";
import { Tenant } from "@/requests/tenant";
import { useDateFormat } from "@vueuse/core";
const { t, locale } = useI18n();

defineProps<{
  itemsPerPage: number;
  items: Array<any>;
  total: number;
  loading: boolean;
  headers: Array<any>;
}>();

const emit = defineEmits<{
  (e: "delete", id: string | null): void;
  (e: "activate", id: string): void;
  (e: "deactivate", id: string): void;
}>();

const tenantId: Ref<string | null> = ref(null);
const showDeleteConfirmDialog = ref(false);
const dialogTitle: Ref<string | null> = ref(null);
const dialogMessage: Ref<string | null> = ref(null);

const showDeleteConfirmModal = (tenant: Tenant) => {
  tenantId.value = tenant.id;
  dialogTitle.value = t("admin.tenants.notifications.deleteConfirm", {
    name: tenant.name,
  });
  dialogMessage.value = t("admin.tenants.notifications.deleteMessage", {
    name: tenant.name,
  });
  showDeleteConfirmDialog.value = true;
};

const confirmDelete = () => {
  emit("delete", tenantId.value);
};

const abortDelete = () => {
  tenantId.value = null;
  showDeleteConfirmDialog.value = false;
};

const activate = (id: string) => {
  emit("activate", id);
};

const deactivate = (id: string) => {
  emit("deactivate", id);
};
</script>

<template>
  <v-card elevation="6" class="mt-4">
    <v-card-title>
      <toolbar
        :title="t('admin.tenants.list.title')"
        :button="{
          icon: 'bx-plus',
          text: t('shared.new'),
          to: { name: 'tenants-create' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-data-table-server
        :items-per-page="itemsPerPage"
        :items="items"
        :items-length="total"
        :headers="headers"
        :loading="loading"
        class="striped border"
      >
        <template #[`item.id`]="{ item }">
          <router-link
            :to="{ name: 'tenants-view', params: { id: item.id } }"
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
              <v-list-item
                :to="{ name: 'tenants-view', params: { id: item.id } }"
              >
                <v-list-item-title v-text="t('shared.view')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-show" />
                </template>
              </v-list-item>

              <v-list-item
                :to="{ name: 'tenants-edit', params: { id: item.id } }"
              >
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

              <v-list-item
                @click="item.isActive ? deactivate(item.id) : activate(item.id)"
              >
                <v-list-item-title
                  v-text="
                    item.isActive
                      ? t('shared.deactivate')
                      : t('shared.activate')
                  "
                />
                <template v-slot:prepend>
                  <v-icon
                    :icon="item.isActive ? 'bx-toggle-left' : 'bx-toggle-right'"
                  />
                </template>
              </v-list-item>
            </v-list>
          </v-menu>
        </template>
      </v-data-table-server>
    </v-card-text>
  </v-card>

  <confirm-dialog
    v-model="showDeleteConfirmDialog"
    :confirm-button-text="t('shared.confirm')"
    :cancel-button-text="t('shared.cancel')"
    :title="dialogTitle"
    :message="dialogMessage"
    @confirm="confirmDelete"
    @cancel="abortDelete"
  />
</template>
