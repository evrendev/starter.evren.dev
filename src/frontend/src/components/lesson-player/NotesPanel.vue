<script setup lang="ts">
import { useNoteStore } from "@/stores/note";
import { usePersonalStore } from "@/stores/personal";
import { Notify } from "@/stores/notification";
import { CreateNoteRequest } from "@/types/requests/lessonPage";
import { useSanitizedHtml } from "@/composables/useSanitizedHtml";
import { useI18n } from "vue-i18n";
import { useTheme } from "vuetify";
import { format } from "date-fns";
import { enUS, de, tr } from "date-fns/locale";
import type { Locale } from "date-fns";

const props = defineProps<{
  lessonPageId: string;
}>();

const { t, locale } = useI18n();

// Panel is teleported inside the lesson player dialog; bind explicitly to
// theme tokens rather than relying on CSS-cascade defaults (see
// LessonPlayerDialog.vue for the same reasoning).
const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
const panelBg = computed(() => vuetifyTheme.current.value.colors.surface);
const panelFg = computed(() => vuetifyTheme.current.value.colors["on-surface"]);
const mutedFg = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "99");
// Header strip follows the theme (grey-100: #F3F4F6 light / #374151 dark)
const headerBg = computed(() => vuetifyTheme.current.value.colors["grey-100"]);
const noteCardBorder = computed(() => vuetifyTheme.current.value.colors["grey-300"]);
// Red in dark theme is the secondary token (primary is gray there)
const addButtonColor = computed(() => (isDark.value ? "secondary" : "primary"));

const dateLocales: Record<string, Locale> = { en: enUS, de, tr };

const formatNoteDate = (value: Date | string) =>
  format(new Date(value), "P", { locale: dateLocales[locale.value] ?? enUS });
const { sanitize } = useSanitizedHtml();
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
    <v-card class="h-100 d-flex flex-column notes-panel-card">
      <v-card-title class="bg-light panel-header">
        <v-icon icon="bx-note" class="mr-2" />
        {{ t("shared.notes") }}
      </v-card-title>

      <v-card-text class="flex-grow-1 overflow-y-auto">
        <!-- Add New Note Section -->
        <div class="mb-4">
          <v-label class="text-subtitle-2 mb-2 panel-label">
            {{ t("catalog.notes.create.title") }}
          </v-label>
          <v-textarea
            v-model="newNoteContent"
            :placeholder="t('catalog.notes.create.placeholder')"
            rows="4"
            variant="outlined"
            :disabled="submitting || loading"
          />
          <v-btn
            class="mt-2"
            :color="addButtonColor"
            size="small"
            @click="handleAddNote"
            :loading="submitting"
            prepend-icon="bx-plus"
          >
            {{ t("shared.add") }}
          </v-btn>
        </div>

        <v-divider class="my-4" />

        <!-- Notes List Section -->
        <div v-if="notes.length === 0" class="text-center text-caption panel-muted">
          {{ t("shared.noData") }}
        </div>

        <div v-else class="notes-list">
          <v-card
            v-for="note in notes"
            :key="note.id"
            variant="outlined"
            class="mb-3 pa-3 note-card"
          >
            <div class="d-flex justify-space-between align-start">
              <div class="flex-grow-1">
                <div
                  class="text-body2 mb-2 note-content"
                  :innerHTML="sanitize(note.content)"
                  style="word-wrap: break-word; white-space: pre-wrap"
                />
                <div v-if="note.createdOn" class="text-caption panel-muted">
                  {{ formatNoteDate(note.createdOn) }}
                </div>
              </div>
              <v-btn
                icon="bx-trash"
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

.notes-panel-card {
  background-color: v-bind(panelBg);
  color: v-bind(panelFg);
}

.bg-light {
  background-color: v-bind(headerBg);
}

.panel-header,
.panel-label {
  color: v-bind(panelFg);
}

.panel-muted {
  color: v-bind(mutedFg);
}

.note-card {
  background-color: v-bind(panelBg);
  border-color: v-bind(noteCardBorder) !important;
}

.note-content {
  color: v-bind(panelFg);
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
