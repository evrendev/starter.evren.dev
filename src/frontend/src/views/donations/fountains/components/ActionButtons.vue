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

const emit = defineEmits(["showDetails", "changeMediaStatus"]);
const menuOpen = ref(false);

const handleMediaChange = (statusName) => {
  emit("changeMediaStatus", props.donationId, statusName);
  menuOpen.value = false; // Menu kapanır
};
</script>
<template>
  <div class="d-flex ga-2 justify-end">
    <v-btn density="compact" @click.stop="$emit('showDetails', donationId)" prepend-icon="$information" size="small">
      {{ t("admin.donations.info") }}
    </v-btn>

    <v-menu v-model="menuOpen">
      <template #activator="{ props }">
        <v-btn v-bind="props" prepend-icon="$media" append-icon="$chevronDown" density="compact" size="small">
          {{ t("admin.donations.media.title") }}
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
