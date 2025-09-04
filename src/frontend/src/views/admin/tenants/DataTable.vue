<script setup lang="ts">
import StatusIcon from "@/components/admin/StatusIcon.vue";
import { useDateFormat } from "@vueuse/core";
const { t, locale } = useI18n();

defineProps<{
  itemsPerPage: number;
  items: Array<any>;
  total: number;
  loading: boolean;
  headers: Array<any>;
}>();
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

              <v-list-item>
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
</template>
