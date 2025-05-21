<script setup async>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { ParentCard } from "@/components/shared/";

const { t } = useI18n();

defineProps({
  items: {
    type: Array,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
});

const headers = ref([
  { title: t("admin.donations.fountains.fields.info"), key: "info", sortable: true },
  { title: t("admin.donations.fountains.fields.contact"), key: "contact", sortable: true },
  { title: t("admin.donations.fountains.fields.phone"), key: "phone", sortable: false, width: "100px" },
  {
    title: t("admin.donations.fountains.fields.creationDate"),
    key: "creationDate.displayDate",
    align: "center",
    sortable: true,
    width: "100px"
  },
  { title: t("admin.donations.fountains.fields.weeks"), key: "weeks", sortable: false, align: "center", width: "48px" }
]);
</script>

<template>
  <parent-card>
    <v-data-table-server
      :headers="headers"
      :items="items"
      :loading="loading"
      items-length="12"
      hide-default-footer
      class="text-caption striped donations-table"
    >
      <template v-slot:item="{ item }">
        <tr :key="item.id" class="donation-row">
          <td>
            <div class="info-wrapper" v-html="item.htmlBanner" />
          </td>
          <td>
            {{ item.contact }}
          </td>
          <td>
            <v-btn v-if="item.phone" size="x-small" color="success" class="d-block w-100 text-truncate">
              <v-icon icon="$whatsapp" size="small" class="mr-1" />
              {{ item.phone.formattedNumber }}
            </v-btn>
          </td>
          <td>
            {{ item.creationDate.displayDate }}
          </td>
          <td class="text-center">
            <v-chip :class="`bg-${item.status.backgroundColor}`" size="small">
              {{ item.weeks }}
            </v-chip>
          </td>
        </tr>
      </template>
    </v-data-table-server>
  </parent-card>
</template>

<style lang="scss" scoped>
:deep(.donations-table) {
  thead {
    th {
      background-color: rgb(var(--v-theme-lightprimary));

      &:first-child {
        border-radius: 0.25rem 0 0 0.25rem;
      }

      &:last-child {
        border-radius: 0 0.25rem 0.25rem 0;
      }
      span {
        font-size: 0.75rem;
        font-weight: 700;
        text-align: center;
      }
    }
  }
  tbody {
    tr {
      transition: all 0.1s ease;

      &:first-child {
        td {
          padding-top: 16px;
        }
      }

      &:hover {
        position: relative;
        z-index: 10;
        box-shadow:
          0 4px 20px rgba(0, 0, 0, 0.2),
          inset 10px 0 0 0 rgb(var(--v-theme-primary));
        cursor: pointer;
      }

      td {
        .info-wrapper {
          overflow: auto;
          text-overflow: ellipsis;
          white-space: break-spaces;
        }
      }
    }
  }
}
</style>
