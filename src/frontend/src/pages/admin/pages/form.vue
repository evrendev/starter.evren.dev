<script setup lang="ts">
import { Notify } from "@/stores/notification";
import { usePageStore } from "@/stores/page";
import { useChapterStore } from "@/stores/chapter";
import { PageContentType } from "@/models/page";
import { CreatePageRequest, UpdatePageRequest } from "@/types/requests/page";
import { useConvertImportedContent } from "@/composables/useConvertImportedContent";

const { t } = useI18n();
const route = useRoute();
const router = useRouter();

const pageStore = usePageStore();
const chapterStore = useChapterStore();
const { pageDetails, loading } = storeToRefs(pageStore);

const pageId = computed(() => route.params.id as string);
const isEdit = computed(() => !!pageId.value);
const chapterId = computed(
  () => (route.params.chapterId as string) || pageDetails.value?.chapterId || "",
);

// Form state
const form = reactive({
  title: "",
  content: "",
  contentType: PageContentType.Text,
  order: 0,
  mediaUrl: "",
  isImported: false,
});

const contentTypeOptions = computed(() =>
  Object.values(PageContentType).map((value) => ({
    value,
    title: t(`admin.pages.fields.contentType.options.${value.toLowerCase()}`),
  })),
);

const { convertToEditable } = useConvertImportedContent();
const showConvertConfirm = ref(false);

// True once form.isImported reflects the real Page (or immediately in create
// mode, where there's nothing to fetch). Content is only rendered once this is
// true — form.isImported defaults to false, and briefly rendering the
// QuillyEditor branch before the fetch resolves would mount then unmount
// Quill, which injects its toolbar as a raw DOM sibling outside Vue's own
// vnode tree; Vue has no record of it and leaves it orphaned in the DOM when
// the branch switches back to the iframe view.
const contentReady = ref(false);

const handleConvertConfirmed = () => {
  form.content = convertToEditable(form.content);
  // Local-only: nothing is persisted until Save is pressed. Flipping this
  // switches the template's v-if from the iframe+textarea view to the real
  // QuillyEditor so the admin can immediately see/edit the converted result.
  form.isImported = false;
};

onMounted(async () => {
  if (isEdit.value && pageId.value) {
    const result = await pageStore.getPageById(pageId.value);
    if (result.succeeded && pageDetails.value) {
      form.title = pageDetails.value.title;
      form.content = pageDetails.value.content || "";
      form.contentType = (pageDetails.value.contentType as PageContentType) || PageContentType.Text;
      form.order = pageDetails.value.order || 0;
      form.mediaUrl = pageDetails.value.mediaUrl || "";
      form.isImported = pageDetails.value.isImported || false;
    }
  } else if (route.params.chapterId) {
    await chapterStore.getById(route.params.chapterId as string);
  }
  contentReady.value = true;
});

const pageTitle = computed(() => {
  if (isEdit.value) {
    return t("admin.pages.edit.title");
  }
  return t("admin.pages.create.title");
});

// chapterId isn't known yet on the initial render of the edit route (it only
// comes from the Page once getPageById resolves) — an empty params.id would
// make vue-router throw "Missing required param", so fall back to the
// chapter list until it's available
const backTo = computed(() =>
  chapterId.value
    ? { name: "chapter-view", params: { id: chapterId.value } }
    : { name: "chapter-list" },
);

const breadcrumbs = computed(() => [
  {
    title: t("admin.components.breadcrumbs.admin.title"),
    to: { name: "dashboard" },
  },
  {
    title: t("admin.components.breadcrumbs.admin.chapters"),
    to: { path: "/admin/chapters" },
  },
  {
    title: chapterStore.chapter?.title || "",
    to: backTo.value,
  },
  {
    title: isEdit.value ? t("shared.edit") : t("shared.create"),
    disabled: true,
  },
]);

const handleSubmit = async () => {
  try {
    let response;

    if (isEdit.value && pageId.value) {
      const payload: UpdatePageRequest = {
        id: pageId.value,
        chapterId: chapterId.value,
        title: form.title,
        content: form.content,
        contentType: form.contentType,
        order: form.order,
        mediaUrl: form.mediaUrl || undefined,
        isImported: form.isImported,
      };
      response = await pageStore.updatePage(pageId.value, payload);
    } else {
      const payload: CreatePageRequest = {
        chapterId: chapterId.value,
        title: form.title,
        content: form.content,
        contentType: form.contentType,
        order: form.order,
        mediaUrl: form.mediaUrl || undefined,
      };
      response = await pageStore.createPage(payload);
    }

    if (response.succeeded) {
      const msg = isEdit.value
        ? t("admin.pages.notifications.updated")
        : t("admin.pages.notifications.created");
      Notify.success(msg);
      await router.push(backTo.value);
    } else {
      Notify.error(t("admin.pages.notifications.saveFailed"));
    }
  } catch (error) {
    Notify.error(t("shared.unexpectedError"));
  }
};
</script>

<template>
  <breadcrumb :items="breadcrumbs" />

  <v-container>
    <v-card>
      <v-card-title>{{ pageTitle }}</v-card-title>

      <v-card-text>
        <v-form @submit.prevent="handleSubmit">
          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model="form.title"
                :label="t('admin.pages.fields.title.title')"
                :placeholder="t('admin.pages.fields.title.placeholder')"
                outlined
                required
              />
            </v-col>

            <v-col cols="12" md="6">
              <v-select
                v-model="form.contentType"
                :items="contentTypeOptions"
                item-title="title"
                item-value="value"
                :label="t('admin.pages.fields.contentType.title')"
                outlined
              />
            </v-col>
          </v-row>

          <v-row>
            <v-col cols="12" md="6">
              <v-text-field
                v-model.number="form.order"
                :label="t('admin.pages.fields.order.title')"
                type="number"
                outlined
              />
            </v-col>

            <v-col cols="12" md="6">
              <v-text-field
                v-model="form.mediaUrl"
                :label="t('admin.pages.fields.mediaUrl.title')"
                :placeholder="t('admin.pages.fields.mediaUrl.placeholder')"
                outlined
              />
            </v-col>
          </v-row>

          <v-row>
            <v-col cols="12">
              <label class="text-subtitle-2 mb-2">
                {{ t("admin.pages.fields.content.title") }}
              </label>

              <template v-if="contentReady">
                <template v-if="form.isImported">
                  <v-alert type="info" variant="tonal" density="compact" class="mb-2">
                    {{ t("admin.pages.fields.content.importedNotice") }}
                  </v-alert>
                  <v-btn
                    color="warning"
                    variant="tonal"
                    size="small"
                    prepend-icon="bx-edit-alt"
                    class="mb-4"
                    @click="showConvertConfirm = true"
                  >
                    {{ t("admin.pages.actions.convertToEditable") }}
                  </v-btn>
                  <label class="text-subtitle-2 mb-2 d-block">
                    {{ t("admin.pages.fields.content.richPreview") }}
                  </label>
                  <iframe
                    :srcdoc="form.content"
                    sandbox="allow-same-origin"
                    class="imported-content-preview"
                    :title="form.title"
                  />
                  <label class="text-subtitle-2 mt-4 mb-2 d-block">
                    {{ t("admin.pages.fields.content.rawHtml") }}
                  </label>
                  <v-textarea
                    v-model="form.content"
                    variant="outlined"
                    rows="10"
                    no-resize
                    spellcheck="false"
                    class="raw-html-editor"
                  />
                </template>

                <QuillyEditor
                  v-else
                  v-model="form.content"
                />
              </template>
            </v-col>
          </v-row>

          <v-row class="mt-4">
            <v-col cols="12" class="d-flex gap-2">
              <v-btn
                type="submit"
                color="primary"
                :loading="loading"
              >
                {{ isEdit ? t("shared.save") : t("shared.create") }}
              </v-btn>
              <v-btn
                :to="backTo"
                variant="outlined"
              >
                {{ t("shared.cancel") }}
              </v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
    </v-card>
  </v-container>

  <confirm-dialog
    v-model:show-dialog="showConvertConfirm"
    :title="t('admin.pages.actions.convertToEditableConfirm.title')"
    :message="t('admin.pages.actions.convertToEditableConfirm.message')"
    :confirm-button-text="t('admin.pages.actions.convertToEditableConfirm.confirm')"
    :cancel-button-text="t('shared.cancel')"
    confirm-button-color="warning"
    @confirm="handleConvertConfirmed"
  />
</template>

<style scoped>
.imported-content-preview {
  width: 100%;
  aspect-ratio: 960 / 540;
  border: 1px solid rgba(var(--v-border-color), var(--v-border-opacity));
  border-radius: 4px;
  background: #fff;
}

.raw-html-editor :deep(textarea) {
  font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
  font-size: 13px;
}
</style>
