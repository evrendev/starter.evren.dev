<script setup>
import { useI18n } from "vue-i18n";
import { useAppStore } from "@/stores";
import html2pdf from "html2pdf.js";
import ReportTable from "./ReportTable.vue";

const { t } = useI18n();

const appStore = useAppStore();

const props = defineProps({
  projects: {
    type: Array,
    default: () => []
  },
  isoYear: {
    type: Number,
    default: new Date().getFullYear(),
    required: false
  },
  isoWeekNumber: {
    type: Number,
    default: 1,
    required: false
  }
});

const exportToPDF = async () => {
  appStore.setLoading(true);

  try {
    await html2pdf()
      .from(document.getElementById("weekly-report-container"))
      .set({
        margin: 15,
        filename: `${props.isoYear}-KW${props.isoWeekNumber}.pdf`
      })
      .save();
  } catch (e) {
    console.error("PDF export failed:", e);
  } finally {
    appStore.setLoading(false);
  }
};
</script>
<template>
  <v-row>
    <v-col md="12">
      <v-sheet class="pa-2">
        <v-card :title="`${isoYear}-KW${isoWeekNumber}`" class="text-center" elevation="0">
          <template v-slot:append>
            <v-btn
              color="success"
              size="small"
              prepend-icon="$filePdfBox"
              :text="t('common.createPdf')"
              elevation="1"
              class="py-2"
              @click.stop="exportToPDF"
            />
          </template>
        </v-card>
        <div id="weekly-report-container">
          <report-table v-for="item in projects" :key="item.project.name" :item="item" />
        </div>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<style lang="scss" scoped></style>
