<script setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps({
  donationId: {
    type: String,
    default: null,
    required: true
  },
  mediaStatuses: {
    type: Array,
    default: () => [],
    required: true
  }
});

const emit = defineEmits(["showAllInformation", "showConfirmDialog", "changeMediaStatus"]);
const mediaMenuOpen = ref(false);

const handleMediaChange = (statusName) => {
  emit("changeMediaStatus", props.donationId, statusName);
  mediaMenuOpen.value = false;
};
</script>
<template>
  <div class="d-flex ga-2 justify-end">
    <v-btn icon density="compact" @click.stop="$emit('showAllInformation', donationId)">
      <v-icon size="x-small">$information</v-icon>
      <v-tooltip activator="parent" location="top">
        {{ t("common.showDetails") }}
      </v-tooltip>
    </v-btn>

    <v-btn icon density="compact" @click.stop="$emit('showConfirmDialog', donationId)">
      <v-icon size="x-small">$trashCan</v-icon>
      <v-tooltip activator="parent" location="top">
        {{ t("common.delete") }}
      </v-tooltip>
    </v-btn>

    <v-menu v-model="mediaMenuOpen">
      <template #activator="{ props }">
        <v-btn icon v-bind="props" density="compact">
          <v-icon size="x-small">$media</v-icon>
          <v-tooltip activator="parent" location="top">
            {{ t("admin.donations.media.change") }}
          </v-tooltip>
        </v-btn>
      </template>

      <v-list density="compact">
        <v-list-item
          v-for="status in mediaStatuses"
          :key="status.name"
          :value="status.name"
          @click.stop="handleMediaChange(status.name)"
          class="d-flex ga-2"
        >
          <template v-slot:prepend>
            <div
              :class="`bg-${status.backgroundColor}`"
              :style="{
                width: '12px',
                height: '12px',
                borderRadius: '50%'
              }"
            />
          </template>
          <v-list-item-title>
            {{ t(`admin.donations.media.status.${status.name}`) }}
          </v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>
  </div>
</template>
