<script setup lang="ts">
import { Chapter } from "@/models/chapter";
import { Props } from "@/types/requests/app";
const { t } = useI18n();

withDefaults(defineProps<Props<Chapter>>(), {
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

const chapterId: Ref<string> = ref("");
const toggleDeleteConfirmDialog = ref(false);
const dialogTitle: Ref<string | null> = ref(null);
const dialogMessage: Ref<string | null> = ref(null);

const showDeleteConfirmModal = (chapter: Chapter) => {
  chapterId.value = chapter.id;
  dialogTitle.value = t("admin.chapters.notifications.deleteConfirm", {
    title: chapter.title,
  });
  dialogMessage.value = t("admin.chapters.notifications.deleteMessage", {
    title: chapter.title,
  });
  toggleDeleteConfirmDialog.value = true;
};

const confirmDelete = () => {
  emit("delete", chapterId.value);
};

const abortDelete = () => {
  chapterId.value = "";
  toggleDeleteConfirmDialog.value = false;
};
</script>

<template>
  <v-card elevation="6" class="mt-4">
    <v-card-title>
      <toolbar
        color="success"
        :title="t('admin.chapters.list.title')"
        :button="{
          icon: 'bx-plus',
          text: t('shared.new'),
          to: { name: 'chapter-create' },
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
        :fixed-header="true"
        @update:options="emit('update:options', $event)"
        class="striped border"
      >
        <template #[`item.courseTitle`]="{ item }">
          <router-link
            v-if="item.courseId"
            :to="{ name: 'course-view', params: { id: item.courseId } }"
          >
            {{ item.courseTitle }}
          </router-link>
        </template>

        <template #[`item.title`]="{ item }">
          <router-link
            v-if="item.id"
            :to="{ name: 'chapter-view', params: { id: item.id } }"
          >
            {{ item.title }}
          </router-link>
        </template>

        <template #[`item.description`]="{ item }">
          <span class="text-pre-wrap">
            {{ item.description }}
          </span>
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
                v-if="item.id"
                :to="{ name: 'chapter-view', params: { id: item.id } }"
              >
                <v-list-item-title v-text="t('shared.view')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-show" />
                </template>
              </v-list-item>

              <v-list-item
                v-if="item.id"
                :to="{ name: 'chapter-edit', params: { id: item.id } }"
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

<style lang="scss" scoped>
@media screen and (max-width: 960px) {
  :deep(.text-pre-wrap) {
    display: block;
    width: 200px;
    white-space: nowrap !important;
    overflow: hidden;
    text-overflow: ellipsis;
  }
}
</style>
