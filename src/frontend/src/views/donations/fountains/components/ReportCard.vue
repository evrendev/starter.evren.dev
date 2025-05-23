<script setup>
import { useI18n } from "vue-i18n";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";
import ReportTable from "./ReportTable.vue";

const { t } = useI18n();

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

const exportToPDF = () => {
  const doc = new jsPDF("p", "mm", "a4");
  const marginTop = 14;
  let currentY = marginTop;

  doc.setFontSize(16);
  doc.setFont("helvetica", "bold");
  doc.text(`${props.isoYear}-KW${props.isoWeekNumber}`, 105, currentY, { align: "center" });
  currentY += 10;

  props.projects.forEach((project, idx) => {
    const body = [];

    const section = (label, items, color) => {
      const labelText = t(`admin.donations.fountains.weeklyReport.${label}`);

      if (!items || items.length === 0) {
        body.push([
          { content: labelText },
          {
            content: t("common.none"),
            styles: { fillColor: color }
          }
        ]);
        return;
      }

      const lines = items
        .map((item) => `${item.code} (${item.date} - ${item.weeks} ${t("admin.donations.fountains.weeklyReport.weeks")})`)
        .join("\n");

      body.push([
        { content: labelText },
        {
          content: lines,
          styles: {
            fillColor: color,
            cellPadding: { top: 3, left: 3, bottom: 3 },
            halign: "left",
            valign: "top"
          }
        }
      ]);
    };

    body.unshift([
      {
        content: project.project.name,
        colSpan: 2,
        styles: {
          halign: "center",
          fontStyle: "bold",
          fontSize: 12,
          fillColor: [245, 245, 245],
          textColor: 40
        }
      }
    ]);

    section("lastOnlineFountain", [project.lastOnlineFountain], [212, 237, 218]);
    section("pendingMediaFountains", project.pendingMediaFountains, [255, 243, 205]);
    section("lastAssignedFountain", [project.lastAssignedFountain], [255, 243, 205]);
    section("missingSince6Weeks", project.missingSince6Weeks, [248, 215, 218]);
    section("missingSince8Weeks", project.missingSince8Weeks, [245, 198, 203]);
    section("missingSince13Weeks", project.missingSince13Weeks, [241, 176, 183]);

    autoTable(doc, {
      startY: currentY,
      head: [],
      body,
      styles: {
        fontSize: 9,
        cellPadding: 3,
        overflow: "linebreak",
        valign: "middle",
        lineWidth: 0.1,
        lineColor: 150
      },
      margin: { left: 14, right: 14 },
      didDrawPage: (data) => {
        currentY = data.cursor.y + 10;
      }
    });

    if (idx < props.projects.length - 1) {
      doc.addPage();
      currentY = marginTop;
    }
  });

  doc.save(`${props.isoYear}-KW${props.isoWeekNumber}.pdf`);
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
          <report-table v-for="item in projects" :key="item.project.name" :item="item" class="report-section" />
        </div>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<style lang="scss" scoped>
#weekly-report-container {
  page-break-inside: avoid;

  .report-section {
    break-inside: avoid;
    page-break-inside: avoid;
    page-break-after: auto;
    margin-bottom: 20px;
  }
}
</style>
