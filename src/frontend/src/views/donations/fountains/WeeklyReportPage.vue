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
const fountainDonationStore = useFountainDonationStore();
const { loading } = storeToRefs(appStore);
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
        <v-table density="compact" class="mb-4 border rounded-sm" v-for="report in reports" :key="report.project.name">
          <thead>
            <tr class="bg-lightprimary">
              <th colspan="2" class="text-center">
                <h3>{{ report.project.name }}</h3>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>
                <span class="text-no-wrap text-subtitle-1"> Letzter geschnittener Brunnen (Online): </span>
              </td>
              <td class="bg-lightsuccess">
                <div class="fountain-item">
                  <code>
                    {{ report.lastOnlineFountain.code }}
                  </code>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2" v-if="report.lastOnlineFountain">
                    ({{ report.lastOnlineFountain.date }} - {{ report.lastOnlineFountain.weeks }} Woche)
                  </span>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2" v-else> (Kein Brunnen) </span>
                </div>
              </td>
            </tr>
            <tr>
              <td>
                <span class="text-no-wrap text-subtitle-1">
                  Aktueller nicht geschnittener Brunnen:
                  <br />
                  Die Brunnen die im Nas sind*
                </span>
              </td>
              <td v-if="report.pendingMediaFountains.length > 0" class="bg-lightwarning">
                <div class="fountain-item" v-for="item in report.pendingMediaFountains" :key="item.code">
                  <code>
                    {{ item.code }}
                  </code>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2">
                    ({{ item.date }} - {{ item.weeks }} Woche)
                  </span>
                </div>
              </td>
              <td v-else class="bg-lightwarning">
                <code> - </code>
              </td>
            </tr>
            <tr>
              <td>
                <span class="text-no-wrap text-subtitle-1"> Letzter Vergebener Brunnencode: </span>
              </td>
              <td class="bg-lightwarning">
                <div class="fountain-item">
                  <code>
                    {{ report.lastAssignedFountainCode.code }}
                  </code>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2" v-if="report.lastAssignedFountainCode">
                    ({{ report.lastAssignedFountainCode.date }} - {{ report.lastAssignedFountainCode.weeks }} Woche)
                  </span>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2" v-else> (Kein Brunnen) </span>
                </div>
              </td>
            </tr>
            <tr>
              <td>
                <span class="text-no-wrap text-subtitle-1"> Ordner/Dateien die fehlen ab 6 Woche: </span>
              </td>
              <td v-if="report.missingSince6Weeks.length > 0" class="bg-lighterror">
                <div class="fountain-item" v-for="item in report.missingSince6Weeks" :key="item.code">
                  <code>
                    {{ item.code }}
                  </code>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2"
                    >({{ item.date }} - {{ item.weeks }} Woche)</span
                  >
                </div>
              </td>
              <td v-else class="bg-lighterror"><span class="text-error"> Keine </span></td>
            </tr>
            <tr>
              <td>
                <span class="text-no-wrap text-subtitle-1"> Ordner/Dateien die fehlen ab 8 Woche </span>
              </td>
              <td v-if="report.missingSince8Weeks.length > 0" class="bg-lighterror">
                <div class="fountain-item" v-for="item in report.missingSince8Weeks" :key="item.code">
                  <code>
                    {{ item.code }}
                  </code>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2">
                    ({{ item.date }} - {{ item.weeks }} Woche)
                  </span>
                </div>
              </td>
              <td v-else class="bg-lighterror"><span class="text-error"> Keine </span></td>
            </tr>
            <tr>
              <td>
                <span class="text-no-wrap text-subtitle-1"> Nicht gut in der Zeit ab 13 Woche: </span>
              </td>
              <td v-if="report.missingSince13Weeks.length > 0" class="bg-lighterror">
                <div class="fountain-item" v-for="item in report.missingSince8Weeks" :key="item.code">
                  <code>
                    {{ item.code }}
                  </code>
                  <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2">
                    ({{ item.date }} - {{ item.weeks }} Woche)
                  </span>
                </div>
              </td>
              <td v-else class="bg-lighterror"><span class="text-error"> Keine </span></td>
            </tr>
          </tbody>
        </v-table>
      </v-sheet>
    </v-col>
  </v-row>
</template>
