<script setup>
import { shallowRef, onMounted } from "vue";
import { useI18n } from "vue-i18n";
import { useFountainDonationStore, useAppStore } from "@/stores";
import { storeToRefs } from "pinia";
import { Breadcrumb } from "@/components/forms";

const { t } = useI18n();

const breadcrumbs = shallowRef([
  {
    title: t("admin.donations.title"),
    disabled: true,
    href: "/admin/donations"
  },
  {
    title: t("admin.donations.fountains.title"),
    disabled: true,
    href: "#"
  },
  {
    title: t("admin.donations.fountains.weeklyReport.title"),
    disabled: true,
    href: "#"
  }
]);

const appStore = useAppStore();
const { loading } = storeToRefs(appStore);
const fountainDonationStore = useFountainDonationStore();
const { reports } = storeToRefs(fountainDonationStore);

onMounted(() => {
  fountainDonationStore.getWeeklyReports();
});
</script>

<template>
  <breadcrumb :title="t('admin.donations.fountains.weeklyReport.title')" :breadcrumbs="breadcrumbs" />
  <v-row>
    <v-col md="12" v-show="!loading">
      <v-sheet class="pa-2">
        <v-table density="compact" class="mb-4" v-for="report in reports" :key="report.project.name">
          <thead>
            <tr>
              <th colspan="2" class="text-center">
                <h3>{{ report.project.name }}</h3>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Letzter geschnittener Brunnen (Online):</td>
              <td>
                {{ report.lastOnlineFountain.code }}
                ({{ report.lastOnlineFountain.date }} - {{ report.lastOnlineFountain.weeks }} Woche)
              </td>
            </tr>
            <tr>
              <td>
                Aktueller nicht geschnittener Brunnen:
                <br />
                Die Brunnen die im Nas sind*
              </td>
              <td v-if="report.pendingMediaFountains.length > 0">
                <div class="fountain-item" v-for="item in report.pendingMediaFountains" :key="item.code">
                  <span class="code">
                    {{ item.code }}
                  </span>
                  <span class="date">({{ item.date }} - {{ item.weeks }} Woche)</span>
                </div>
              </td>
            </tr>
            <tr>
              <td>Letzter Vergebener Brunnencode:</td>
              <td>
                {{ report.lastAssignedFountainCode.code }}
                ({{ report.lastAssignedFountainCode.date }} - {{ report.lastAssignedFountainCode.weeks }} Woche)
              </td>
            </tr>
            <tr>
              <td>Ordner/Dateien die fehlen ab 6 Woche:</td>
              <td v-if="report.missingSince6Weeks.length > 0">
                <div class="fountain-item" v-for="item in report.missingSince6Weeks" :key="item.code">
                  <span class="code">
                    {{ item.code }}
                  </span>
                  <span class="date">({{ item.date }} - {{ item.weeks }} Woche)</span>
                </div>
              </td>
              <td v-else>Keine</td>
            </tr>
            <tr>
              <td>Ordner/Dateien die fehlen ab 8 Woche</td>
              <td v-if="report.missingSince8Weeks.length > 0">
                <div class="fountain-item" v-for="item in report.missingSince8Weeks" :key="item.code">
                  <span class="code">
                    {{ item.code }}
                  </span>
                  <span class="date">({{ item.date }} - {{ item.weeks }} Woche)</span>
                </div>
              </td>
              <td v-else>Keine</td>
            </tr>
            <tr>
              <td>Nicht gut in der Zeit ab 13 Woche:</td>
              <td v-if="report.missingSince13Weeks.length > 0">
                <div class="fountain-item" v-for="item in report.missingSince8Weeks" :key="item.code">
                  <span class="code">
                    {{ item.code }}
                  </span>
                  <span class="date">({{ item.date }} - {{ item.weeks }} Woche)</span>
                </div>
              </td>
              <td v-else>Keine</td>
            </tr>
          </tbody>
        </v-table>
      </v-sheet>
    </v-col>
  </v-row>
</template>

<style lang="scss" scoped>
.v-table {
  font-size: 14px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  overflow: hidden;

  thead {
    background-color: #f5f7fa;

    th {
      text-align: center;
      font-weight: 600;
      font-size: 16px;
      color: #333;
      padding: 12px;
    }
  }

  tbody {
    tr {
      transition: background-color 0.2s;

      &:hover {
        background-color: #f9f9f9;
      }

      td {
        padding: 10px 14px;
        border-bottom: 1px solid #f0f0f0;

        .fountain-item {
          display: flex;
          align-items: center;
          padding: 4px 0;
          font-size: 14px;
          width: fit-content;
          gap: 4px;

          .code {
            font-weight: 500;
            color: #2c3e50;
            font-family: monospace;
          }

          .date {
            color: #7f8c8d;
            font-size: 13px;
            white-space: nowrap;
          }
        }

        &:first-child {
          font-weight: 500;
          color: #444;
          width: 35%;
          white-space: pre-line;
        }

        &:last-child {
          color: #2c3e50;
        }
      }

      &:last-child td {
        border-bottom: none;
      }
    }
  }
}
</style>
