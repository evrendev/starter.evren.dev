<script setup lang="ts">
import StatusIcon from "@/components/admin/StatusIcon.vue";
import { StudentSummary } from "@/models/student";
import { Props } from "@/types/requests/app";
const { t } = useI18n();

withDefaults(defineProps<Props<StudentSummary>>(), {
  itemsPerPage: 25,
  items: () => [],
  total: 0,
  loading: false,
  headers: () => [],
});

const emit = defineEmits<{
  (e: "toggle-status", student: StudentSummary): void;
  (e: "update:options", options: any): void;
}>();
</script>

<template>
  <v-card elevation="6" class="mt-4">
    <v-card-title>
      <v-col>
        <v-toolbar
          :title="t('admin.students.list.title')"
          density="compact"
          color="surface"
        />
      </v-col>
    </v-card-title>
    <v-card-text>
      <v-data-table-server
        :items-per-page="itemsPerPage"
        :items="items"
        :items-length="total"
        item-value="userId"
        :headers="headers"
        :loading="loading"
        @update:options="emit('update:options', $event)"
        class="striped border"
      >
        <template #[`item.isActive`]="{ item }">
          <status-icon :isActive="item.isActive" />
        </template>

        <template #[`item.fullName`]="{ item }">
          <router-link
            v-if="item.userId"
            :to="{ name: 'student-view', params: { id: item.userId } }"
          >
            {{ item.fullName || item.email }}
          </router-link>
        </template>

        <template #[`item.totalPaid`]="{ item }">
          {{ item.totalPaid.toFixed(2) }} €
        </template>

        <template #[`item.averageCompletionPercent`]="{ item }">
          {{ Math.round(item.averageCompletionPercent) }}%
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
                :to="{ name: 'student-view', params: { id: item.userId } }"
              >
                <v-list-item-title v-text="t('shared.view')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-show" />
                </template>
              </v-list-item>

              <v-list-item @click="emit('toggle-status', item)">
                <v-list-item-title
                  v-text="
                    item.isActive
                      ? t('admin.students.actions.deactivate')
                      : t('admin.students.actions.activate')
                  "
                />
                <template v-slot:prepend>
                  <v-icon
                    :icon="item.isActive ? 'bx-block' : 'bx-check-circle'"
                  />
                </template>
              </v-list-item>
            </v-list>
          </v-menu>
        </template>
      </v-data-table-server>
    </v-card-text>
  </v-card>
</template>
