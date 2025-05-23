<script setup>
import { useI18n } from "vue-i18n";
const { t } = useI18n();

defineProps({
  item: {
    type: Object,
    default: () => ({})
  }
});
</script>
<template>
  <v-table density="compact" class="mb-4 mt-4 border rounded-sm">
    <thead>
      <tr class="bg-lightprimary">
        <th colspan="2" class="text-center">
          <h3>{{ item.project.name }}</h3>
        </th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td class="w-50">
          <span class="text-no-wrap text-subtitle-1">
            {{ t("admin.donations.fountains.weeklyReport.lastOnlineFountain") }}
            <v-chip color="success" size="x-small" class="border" elevation="2"> ({{ t("admin.donations.media.status.online") }}) </v-chip>
          </span>
        </td>
        <td class="bg-lightsuccess">
          <div class="fountain-item" v-if="item.lastOnlineFountain">
            <code>
              {{ item.lastOnlineFountain.code }}
            </code>
            <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2 ml-2">
              ({{ item.lastOnlineFountain.date }} - {{ item.lastOnlineFountain.weeks }}
              {{ t("admin.donations.fountains.weeklyReport.weeks") }})
            </span>
          </div>
          <div class="fountain-item" v-else>
            <code> - </code>
          </div>
        </td>
      </tr>
      <tr>
        <td class="w-50">
          <span class="text-no-wrap text-subtitle-1">
            {{ t("admin.donations.fountains.weeklyReport.pendingMediaFountains") }}
            <br />
            {{ t("admin.donations.fountains.weeklyReport.thoseInNAS") }}
            <span class="text-error">*</span>
          </span>
        </td>
        <td v-if="item.pendingMediaFountains.length > 0" class="bg-lightwarning">
          <div class="fountain-item" v-for="item in item.pendingMediaFountains" :key="item.code">
            <code>
              {{ item.code }}
            </code>
            <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2 ml-2">
              ({{ item.date }} - {{ item.weeks }} {{ t("admin.donations.fountains.weeklyReport.weeks") }})
            </span>
          </div>
        </td>
        <td v-else class="bg-lightwarning">
          <code> - </code>
        </td>
      </tr>
      <tr>
        <td class="w-50">
          <span class="text-no-wrap text-subtitle-1">
            {{ t("admin.donations.fountains.weeklyReport.lastAssignedFountain") }}
          </span>
        </td>
        <td class="bg-lightwarning">
          <div class="fountain-item" v-if="item.lastOnlineFountain">
            <code>
              {{ item.lastAssignedFountain.code }}
            </code>
            <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2 ml-2">
              ({{ item.lastAssignedFountain.date }} - {{ item.lastAssignedFountain.weeks }}
              {{ t("admin.donations.fountains.weeklyReport.weeks") }})
            </span>
          </div>
          <div class="fountain-item" v-else>
            <code> - </code>
          </div>
        </td>
      </tr>
      <tr>
        <td class="w-50">
          <span class="text-no-wrap text-subtitle-1">
            {{ t("admin.donations.fountains.weeklyReport.missingSince6Weeks") }}
          </span>
        </td>
        <td v-if="item.missingSince6Weeks.length > 0" class="bg-lighterror">
          <div class="fountain-item" v-for="item in item.missingSince6Weeks" :key="item.code">
            <code>
              {{ item.code }}
            </code>
            <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2 ml-2">
              ({{ item.date }} - {{ item.weeks }} {{ t("admin.donations.fountains.weeklyReport.weeks") }})
            </span>
          </div>
        </td>
        <td v-else class="bg-lighterror">
          <span class="text-error"> {{ t("common.none") }} </span>
        </td>
      </tr>
      <tr>
        <td class="w-50">
          <span class="text-no-wrap text-subtitle-1">
            {{ t("admin.donations.fountains.weeklyReport.missingSince8Weeks") }}
          </span>
        </td>
        <td v-if="item.missingSince8Weeks.length > 0" class="bg-lighterror">
          <div class="fountain-item" v-for="item in item.missingSince8Weeks" :key="item.code">
            <code>
              {{ item.code }}
            </code>
            <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2 ml-2">
              ({{ item.date }} - {{ item.weeks }} {{ t("admin.donations.fountains.weeklyReport.weeks") }})
            </span>
          </div>
        </td>
        <td v-else class="bg-lighterror">
          <span class="text-error"> {{ t("common.none") }} </span>
        </td>
      </tr>
      <tr>
        <td class="w-50">
          <span class="text-no-wrap text-subtitle-1">
            {{ t("admin.donations.fountains.weeklyReport.missingSince13Weeks") }}
          </span>
        </td>
        <td v-if="item.missingSince13Weeks.length > 0" class="bg-lighterror">
          <div class="fountain-item" v-for="item in item.missingSince13Weeks" :key="item.code">
            <code>
              {{ item.code }}
            </code>
            <span class="text-no-wrap font-weight-light text-medium-emphasis text-subtitle-2 ml-2">
              ({{ item.date }} - {{ item.weeks }} {{ t("admin.donations.fountains.weeklyReport.weeks") }})
            </span>
          </div>
        </td>
        <td v-else class="bg-lighterror">
          <span class="text-error"> {{ t("common.none") }} </span>
        </td>
      </tr>
    </tbody>
  </v-table>
</template>

<style lang="scss" scoped></style>
