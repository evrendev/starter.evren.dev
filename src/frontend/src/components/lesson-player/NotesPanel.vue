<script setup lang="ts">
import { useNoteStore } from "@/stores/note";
import { usePersonalStore } from "@/stores/personal";
import { Notify } from "@/stores/notification";
import { CreateNoteRequest } from "@/types/requests/page";
import { useSanitizedHtml } from "@/composables/useSanitizedHtml";
import { useI18n } from "vue-i18n";
import { useTheme } from "vuetify";
import { format } from "date-fns";
import { enUS, de, tr } from "date-fns/locale";
import type { Locale } from "date-fns";

const props = defineProps<{
  pageId: string;
}>();

const { t, locale } = useI18n();

// Panel is teleported inside the lesson player dialog; bind explicitly to
// theme tokens rather than relying on CSS-cascade defaults (see
// LessonPlayerDialog.vue for the same reasoning).
const vuetifyTheme = useTheme();
const isDark = computed(() => vuetifyTheme.current.value.dark);
// Modal-wide gray unification (light only): matches LessonSidebar/
// LessonPlayerDialog's containerBg. Dark already used "surface" here and
// was already consistent with the rest of the dialog chrome, so it's kept.
const panelBg = computed(() =>
  isDark.value ? vuetifyTheme.current.value.colors.surface : vuetifyTheme.current.value.colors.background,
);
const panelFg = computed(() => vuetifyTheme.current.value.colors["on-surface"]);
const mutedFg = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "99");
// Same gray as the rest of the panel in light (no separate header shade);
// dark keeps its previous grey-100 strip, which wasn't flagged as an issue
const headerBg = computed(() =>
  isDark.value ? vuetifyTheme.current.value.colors["grey-100"] : vuetifyTheme.current.value.colors.background,
);
// Note cards are explicit white/surface, distinct from the now-gray panel
// (light); dark's cards already read fine against dark's own surface tone
const noteCardBg = computed(() => vuetifyTheme.current.value.colors.surface);
const noteCardBorder = computed(() => vuetifyTheme.current.value.colors["grey-300"]);
// Red in dark theme is the secondary token (primary is gray there)
const addButtonColor = computed(() => (isDark.value ? "secondary" : "primary"));
// Same near-black-in-light / on-surface-in-dark treatment as LessonSidebar's
// lesson title, so "Notes" reads with the same weight/darkness
const notesTitleColor = computed(() =>
  isDark.value ? vuetifyTheme.current.value.colors["on-surface"] : vuetifyTheme.current.value.colors["grey-900"],
);

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
  if (props.pageId) {
    await noteStore.getNotesByPage(props.pageId);
  }
});

watch(
  () => props.pageId,
  async (newPageId) => {
    if (newPageId) {
      newNoteContent.value = "";
      await noteStore.getNotesByPage(newPageId);
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
      pageId: props.pageId,
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
      <v-card-title class="bg-light panel-header d-flex align-center">
        <span class="notes-title-text">{{ t("shared.notes") }}</span>
        <v-chip
          size="small"
          :color="addButtonColor"
          variant="flat"
          class="ml-2"
        >
          {{ notes.length }}
        </v-chip>
      </v-card-title>

      <v-card-text class="flex-grow-1 overflow-y-auto">
        <!-- Add New Note Section -->
        <div class="mb-4">
          <v-textarea
            v-model="newNoteContent"
            :placeholder="t('catalog.notes.create.placeholder')"
            rows="4"
            variant="outlined"
            :disabled="submitting || loading"
          />
          <v-btn
            class="mt-2"
            block
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
            :class="{ 'note-card--elevated': !isDark }"
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

.panel-header {
  color: v-bind(panelFg);
}

.notes-title-text {
  font-size: 21px;
  font-weight: 700;
  color: v-bind(notesTitleColor);
}

.panel-muted {
  color: v-bind(mutedFg);
}

.note-card {
  background-color: v-bind(noteCardBg);
  border-color: v-bind(noteCardBorder) !important;
}

/* Outlined v-textarea has a transparent field background by default, which
   let the now-gray panel show through; make it an explicit white/surface
   field to match the rest of the "cards" in the panel */
.notes-panel :deep(.v-textarea .v-field) {
  background-color: v-bind(noteCardBg);
}

/* Light only: dark's note cards already read fine against dark's surface */
.note-card--elevated {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
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
