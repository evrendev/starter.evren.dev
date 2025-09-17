<script setup lang="ts">
import StatusIcon from "@/components/admin/StatusIcon.vue";
import { Tenant } from "@/models/tenant";
import { Props } from "@/types/requests/app";
import { UpgradeTenant } from "@/types/requests/tenant";
import { useDateFormat } from "@vueuse/core";
const { t, locale } = useI18n();

withDefaults(defineProps<Props<Tenant>>(), {
  itemsPerPage: 25,
  items: () => [],
  total: 0,
  loading: false,
  headers: () => [],
});

const emit = defineEmits<{
  (e: "delete", id: string | null): void;
  (e: "activate", id: string): void;
  (e: "deactivate", id: string): void;
  (e: "upgrade", tenant: UpgradeTenant): void;
  (e: "update:options", options: any): void;
}>();

const tenantId: Ref<string> = ref("");
const toggleDeleteConfirmDialog = ref(false);
const toggleUpgradeModalWindow = ref(false);
const dialogTitle: Ref<string | null> = ref(null);
const dialogMessage: Ref<string | null> = ref(null);
const extendedExpiryDate: Ref<string> = ref("");

const showDeleteConfirmModal = (tenant: Tenant) => {
  tenantId.value = tenant.id;
  dialogTitle.value = t("admin.tenants.notifications.deleteConfirm", {
    name: tenant.name,
  });
  dialogMessage.value = t("admin.tenants.notifications.deleteMessage", {
    name: tenant.name,
  });
  toggleDeleteConfirmDialog.value = true;
};

const showUpgradeModal = (tenant: Tenant) => {
  tenantId.value = tenant.id;
  extendedExpiryDate.value = tenant.validUpto
    ? new Date(tenant.validUpto).toISOString().split("T")[0]
    : "";
  toggleUpgradeModalWindow.value = true;
};

const confirmDelete = () => {
  emit("delete", tenantId.value);
};

const abortDelete = () => {
  tenantId.value = "";
  toggleDeleteConfirmDialog.value = false;
};

const activate = (id: string) => {
  emit("activate", id);
};

const deactivate = (id: string) => {
  emit("deactivate", id);
};

const upgrade = () => {
  const tenant: UpgradeTenant = {
    tenantId: tenantId.value,
    extendedExpiryDate: extendedExpiryDate.value,
  };

  emit("upgrade", tenant);
  toggleUpgradeModalWindow.value = false;
};
</script>

<template>
  <v-card elevation="6" class="mt-4">
    <v-card-title>
      <toolbar
        color="success"
        :title="t('admin.tenants.list.title')"
        :button="{
          icon: 'bx-plus',
          text: t('shared.new'),
          to: { name: 'tenant-create' },
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
            :to="{ name: 'tenant-view', params: { id: item.id } }"
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
                :to="{ name: 'tenant-view', params: { id: item.id } }"
              >
                <v-list-item-title v-text="t('shared.view')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-show" />
                </template>
              </v-list-item>

              <v-list-item
                :to="{ name: 'tenant-edit', params: { id: item.id } }"
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

              <v-list-item @click="showUpgradeModal(item)">
                <v-list-item-title v-text="t('shared.upgrade')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-trending-up" />
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

  <modal-window
    :show-modal="toggleUpgradeModalWindow"
    :title="t('admin.tenants.upgrade.title')"
    @update:toggle-modal="() => {}"
  >
    <template #content>
      <v-container>
        <v-row justify="space-around">
          <v-locale-provider :locale="locale">
            <v-date-picker
              color="primary"
              v-model="extendedExpiryDate"
            ></v-date-picker>
          </v-locale-provider>
        </v-row>
      </v-container>
    </template>

    <template #action-buttons>
      <v-btn
        @click="toggleUpgradeModalWindow = false"
        v-text="t('shared.cancel')"
        color="secondary"
      />
      <v-btn @click="upgrade" v-text="t('shared.confirm')" color="primary" />
    </template>
  </modal-window>
</template>
