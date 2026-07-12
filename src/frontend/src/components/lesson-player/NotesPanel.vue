<script setup lang="ts">
import { useNoteStore } from "@/stores/note";
import { usePersonalStore } from "@/stores/personal";
import { Notify } from "@/stores/notification";
import { CreateNoteRequest } from "@/types/requests/lessonPage";
import { useI18n } from "vue-i18n";

const props = defineProps<{
  lessonPageId: string;
}>();

const { t } = useI18n();
const noteStore = useNoteStore();
const personalStore = usePersonalStore();
const { notes, loading } = storeToRefs(noteStore);

const newNoteContent = ref("");
const submitting = ref(false);

onMounted(async () => {
  if (props.lessonPageId) {
    await noteStore.getNotesByLessonPage(props.lessonPageId);
  }
});

watch(
  () => props.lessonPageId,
  async (newLessonPageId) => {
    if (newLessonPageId) {
      newNoteContent.value = "";
      await noteStore.getNotesByLessonPage(newLessonPageId);
    }
  },
);

const handleAddNote = async () => {
  if (!newNoteContent.value.trim()) {
    Notify.warning(t("shared.validation.required"));
    return;
  }

  submitting.value = true;

  try {
    const payload: CreateNoteRequest = {
      lessonPageId: props.lessonPageId,
      userId: personalStore.user?.id || "",
      content: newNoteContent.value,
    };

    const result = await noteStore.createNote(payload);

    if (result.succeeded) {
      Notify.success(t("catalog.notes.create.success"));
      newNoteContent.value = "";
    } else {
      Notify.error(t("catalog.notes.create.failed"));
    }
  } catch (error) {
    Notify.error(t("shared.unexpectedError"));
  } finally {
    submitting.value = false;
  }
};

const handleDeleteNote = async (noteId: string) => {
  if (!confirm(t("shared.confirmDelete"))) return;

  try {
    const result = await noteStore.deleteNote(noteId);

    if (result.succeeded) {
      Notify.success(t("catalog.notes.delete.success"));
    } else {
      Notify.error(t("catalog.notes.delete.failed"));
    }
  } catch (error) {
    Notify.error(t("shared.unexpectedError"));
  }
};
</script>

<template>
  <div class="notes-panel">
    <v-card class="h-100 d-flex flex-column">
      <v-card-title class="bg-light">
        <v-icon icon="mdi-note-text" class="mr-2" />
        {{ t("shared.notes") }}
      </v-card-title>

      <v-card-text class="flex-grow-1 overflow-y-auto">
        <!-- Add New Note Section -->
        <div class="mb-4">
          <v-label class="text-subtitle-2 mb-2">
            {{ t("catalog.notes.create.title") }}
          </v-label>
          <v-textarea
            v-model="newNoteContent"
            :placeholder="t('catalog.notes.create.placeholder')"
            rows="4"
            outlined
            :disabled="submitting || loading"
          />
          <v-btn
            class="mt-2"
            color="primary"
            size="small"
            @click="handleAddNote"
            :loading="submitting"
            prepend-icon="mdi-plus"
          >
            {{ t("shared.add") }}
          </v-btn>
        </div>

        <v-divider class="my-4" />

        <!-- Notes List Section -->
        <div v-if="notes.length === 0" class="text-center text-caption text-grey-7">
          {{ t("shared.noData") }}
        </div>

        <div v-else class="notes-list">
          <v-card
            v-for="note in notes"
            :key="note.id"
            variant="outlined"
            class="mb-3 pa-3"
          >
            <div class="d-flex justify-space-between align-start">
              <div class="flex-grow-1">
                <p class="text-body2 mb-2" style="word-wrap: break-word; white-space: pre-wrap">
                  {{ note.content }}
                </p>
                <div class="text-caption text-grey-7">
                  {{ note.userId }} • {{ new Date(note.completedAt || Date.now()).toLocaleDateString() }}
                </div>
              </div>
              <v-btn
                icon="mdi-delete"
                size="x-small"
                variant="text"
                color="error"
                @click="handleDeleteNote(note.id)"
              />
            </div>
          </v-card>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>

<style scoped>
.notes-panel {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.bg-light {
  background-color: #f5f5f5;
}

.notes-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.overflow-y-auto {
  overflow-y: auto;
  overflow-x: hidden;
}
</style>
