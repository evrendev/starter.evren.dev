<script setup>
import { useI18n } from "vue-i18n";
import { storeToRefs } from "pinia";
import { useAppStore } from "@/stores";

const { t } = useI18n();

const props = defineProps({
  isEdit: {
    type: Boolean,
    default: false
  },
  disabled: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(["reset", "submit"]);

const appStore = useAppStore();
const { loading } = storeToRefs(appStore);

const handleReset = () => {
  emit("reset");
};

const handleSubmit = () => {
  emit("submit");
};
</script>
<template>
  <v-col col="12" sm="4" md="3" class="action-buttons">
    <v-card class="pa-4">
      <v-container :fluid="true">
        <v-row>
          <v-col>
            <v-btn
              block
              color="primary"
              @click="handleSubmit"
              :loading="loading"
              :disabled="disabled"
              :prepend-icon="props.isEdit ? '$pencil' : '$contentSave'"
            >
              {{ props.isEdit ? t("common.update") : t("common.save") }}
            </v-btn>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <v-btn block color="error" :disabled="loading" @click="handleReset" prepend-icon="$refresh">
              {{ t("common.reset") }}
            </v-btn>
          </v-col>
        </v-row>
      </v-container>
    </v-card>
  </v-col>
</template>

<style lang="scss">
.action-buttons {
  position: sticky;
  top: 20px;
}
</style>
