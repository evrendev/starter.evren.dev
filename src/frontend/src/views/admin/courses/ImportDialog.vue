<script lang="ts" setup>
import { Notify } from "@/stores/notification";
import { usePageStore } from "@/stores/page";
import { useChapterStore } from "@/stores/chapter";
import { useJobPolling } from "@/composables/useJobPolling";
import { usePptxSlidesParser } from "@/composables/usePptxSlidesParser";
import {
  ImportJobDto,
  ImportJobFailure,
  ImportStatus,
} from "@/types/responses/importJob";

const { t } = useI18n();

const props = defineProps<{
  modelValue: boolean;
  courseId: string;
}>();

const emit = defineEmits<{
  (e: "update:modelValue", value: boolean): void;
}>();

const pageStore = usePageStore();
const chapterStore = useChapterStore();

const show = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit("update:modelValue", value),
});

const file = ref<File | File[] | null>(null);
const selectedFile = computed<File | null>(() =>
  Array.isArray(file.value) ? (file.value[0] ?? null) : file.value,
);

const importing = ref(false);
const showFailureDetails = ref(false);

const {
  parsing: parsingSlides,
  slidesHtml,
  parse: parseSlides,
  reset: resetSlidesParser,
} = usePptxSlidesParser();

// Kicked off as soon as a file is picked (parallel with the user reading the
// dialog), not on Start click — by the time they click Start it has usually
// already finished. handleStart still awaits it to make sure the freshest
// (or null, on failure) result is what actually gets uploaded.
let parsePromise: Promise<string[] | null> | null = null;
watch(selectedFile, (newFile) => {
  parsePromise = newFile ? parseSlides(newFile) : null;
});

const { status, progress, start, reset } = useJobPolling<ImportJobDto>(
  (jobId) => `/v1/pages/import/${jobId}/status`,
  {
    intervalMs: 1000,
    isTerminal: (s) =>
      s === ImportStatus.Completed || s === ImportStatus.Failed,
  },
);

const isTerminal = computed(
  () =>
    status.value?.status === ImportStatus.Completed ||
    status.value?.status === ImportStatus.Failed,
);

const failures = computed<ImportJobFailure[]>(() => {
  if (!status.value?.errorsJson) return [];
  try {
    return JSON.parse(status.value.errorsJson) as ImportJobFailure[];
  } catch {
    return [];
  }
});

watch(isTerminal, async (terminal) => {
  if (terminal) {
    // A staging chapter may have just been created by the import — refresh the
    // chapter list so it shows up wherever chapters are picked from
    await chapterStore.getAllItems();
  }
});

const handleStart = async () => {
  if (!selectedFile.value) return;

  importing.value = true;
  try {
    if (parsePromise) await parsePromise;

    const result = await pageStore.importPptx(
      props.courseId,
      selectedFile.value,
      slidesHtml.value ?? undefined,
    );

    if (result.succeeded && result.data) {
      await start(result.data);
    } else {
      Notify.error(t("admin.courses.import.startFailed"));
    }
  } finally {
    importing.value = false;
  }
};

const handleClose = () => {
  file.value = null;
  showFailureDetails.value = false;
  parsePromise = null;
  resetSlidesParser();
  reset();
  show.value = false;
};
</script>

<template>
  <v-dialog v-model="show" max-width="600" persistent>
    <v-card>
      <v-card-title class="text-h5 bg-primary">
        {{ t("admin.courses.import.title") }}
      </v-card-title>

      <v-card-text>
        <div v-if="!status">
          <v-file-input
            v-model="file"
            accept=".pptx"
            variant="outlined"
            prepend-icon="bx-file"
            :label="t('admin.courses.import.fileLabel')"
            :disabled="importing"
            hide-details
          />
          <div
            v-if="parsingSlides"
            class="d-flex align-center ga-2 mt-2 text-caption text-medium-emphasis"
          >
            <v-progress-circular indeterminate size="16" width="2" color="primary" />
            <span>{{ t("admin.courses.import.parsingSlides") }}</span>
          </div>
        </div>

        <div v-else>
          <v-progress-linear
            :model-value="progress"
            height="20"
            color="primary"
            rounded
          >
            <template #default="{ value }">
              {{ Math.round(value) }}%
            </template>
          </v-progress-linear>
          <p class="mt-2 text-body-2">
            {{
              t("admin.courses.import.processing", {
                processed: status.processedSlides,
                total: status.totalSlides,
              })
            }}
          </p>

          <v-alert
            v-if="isTerminal"
            :type="status.failedSlides > 0 ? 'warning' : 'success'"
            variant="tonal"
            class="mt-4"
          >
            {{
              t("admin.courses.import.success", {
                count: status.succeededSlides,
              })
            }}

            <div v-if="status.failedSlides > 0" class="mt-2">
              <div class="d-flex align-center ga-2">
                <span>
                  {{
                    t("admin.courses.import.failuresSummary", {
                      count: status.failedSlides,
                    })
                  }}
                </span>
                <v-btn
                  size="small"
                  variant="text"
                  @click="showFailureDetails = !showFailureDetails"
                >
                  {{ t("admin.courses.import.failuresDetails") }}
                </v-btn>
              </div>
              <v-expand-transition>
                <ul v-show="showFailureDetails" class="mt-1">
                  <li v-for="f in failures" :key="f.SlideIndex">
                    {{
                      t("admin.courses.import.failureItem", {
                        index: f.SlideIndex,
                        message: f.Message,
                      })
                    }}
                  </li>
                </ul>
              </v-expand-transition>
            </div>

            <router-link
              class="d-block mt-2"
              :to="{ name: 'chapter-list' }"
              @click="handleClose"
            >
              {{ t("admin.courses.import.viewUnassigned") }}
            </router-link>
          </v-alert>
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn
          v-if="!status"
          color="primary"
          variant="flat"
          :loading="importing"
          :disabled="!selectedFile"
          @click="handleStart"
        >
          {{ t("admin.courses.import.start") }}
        </v-btn>
        <v-btn color="secondary" variant="text" @click="handleClose">
          {{ t("admin.courses.import.close") }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
