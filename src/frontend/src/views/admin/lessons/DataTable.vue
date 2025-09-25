<script setup lang="ts">
import "@fancyapps/ui/dist/fancybox/fancybox.css";
import { Fancybox } from "@fancyapps/ui/dist/fancybox/";
import { Lesson } from "@/models/lesson";
import { Props } from "@/types/requests/app";
const { t } = useI18n();

Fancybox.bind("[data-fancybox]");

withDefaults(defineProps<Props<Lesson>>(), {
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

const baseUrl: Ref<string> = import.meta.env.VITE_APP_BACKEND_BASE_URL;
const lessonId: Ref<string> = ref("");
const toggleDeleteConfirmDialog = ref(false);
const dialogTitle: Ref<string | null> = ref(null);
const dialogMessage: Ref<string | null> = ref(null);

const showDeleteConfirmModal = (lesson: Lesson) => {
  lessonId.value = lesson.id;
  dialogTitle.value = t("admin.lessons.notifications.deleteConfirm", {
    title: lesson.title,
  });
  dialogMessage.value = t("admin.lessons.notifications.deleteMessage", {
    title: lesson.title,
  });
  toggleDeleteConfirmDialog.value = true;
};

const confirmDelete = () => {
  emit("delete", lessonId.value);
};

const abortDelete = () => {
  lessonId.value = "";
  toggleDeleteConfirmDialog.value = false;
};
</script>

<template>
  <v-card elevation="6" class="mt-4">
    <v-card-title>
      <toolbar
        color="success"
        :title="t('admin.lessons.list.title')"
        :button="{
          icon: 'bx-plus',
          text: t('shared.new'),
          to: { name: 'lesson-create' },
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
        <template #[`item.image`]="{ item }">
          <a
            v-if="item.image"
            class="cursor-pointer"
            :data-fancybox="`gallery-${item.chapterId ?? 'no-chapter'}`"
            :data-src="`${baseUrl}/${item.image}`"
            :data-caption="item.title"
          >
            <v-img :src="`${baseUrl}/${item.image}`" size="36" />
          </a>
          <v-icon v-else icon="bx-image" size="36" />
        </template>

        <template #[`item.chapterTitle`]="{ item }">
          <router-link
            v-if="item.chapterId"
            :to="{ name: 'chapter-view', params: { id: item.chapterId } }"
          >
            {{ item.chapterTitle }}
          </router-link>
        </template>

        <template #[`item.title`]="{ item }">
          <router-link
            v-if="item.id"
            :to="{ name: 'lesson-view', params: { id: item.id } }"
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
                :to="{ name: 'lesson-view', params: { id: item.id } }"
              >
                <v-list-item-title v-text="t('shared.view')" />
                <template v-slot:prepend>
                  <v-icon icon="bx-show" />
                </template>
              </v-list-item>

              <v-list-item
                v-if="item.id"
                :to="{ name: 'lesson-edit', params: { id: item.id } }"
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
