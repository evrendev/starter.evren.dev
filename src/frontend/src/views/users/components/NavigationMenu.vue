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

const navigationItems = [
  {
    title: t("admin.users.helpers.information"),
    icon: "$informationBox",
    target: "user-information"
  },
  {
    title: t("admin.users.helpers.permissions"),
    icon: "$shieldAccount",
    target: "user-permissions"
  }
];

const scrollToSection = (target) => {
  const element = document.getElementById(target);
  if (element) {
    const elementPosition = element.getBoundingClientRect().top;
    const offsetPosition = elementPosition + window.pageYOffset - 80;

    window.scrollTo({
      top: offsetPosition,
      behavior: "smooth"
    });
  }
};
</script>
<template>
  <v-col col="12" sm="4" md="3" class="navigation-container">
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

        <v-divider class="my-4"></v-divider>

        <v-row>
          <v-col>
            <h3 class="text-h3 text-center font-weight-thin">
              {{ t("common.navigation") }}
            </h3>
            <v-list density="compact" nav>
              <v-list-item
                v-for="(item, index) in navigationItems"
                :key="index"
                :title="item.title"
                :prepend-icon="item.icon"
                link
                class="mb-1"
                color="primary"
                @click="scrollToSection(item.target)"
              />
            </v-list>
          </v-col>
        </v-row>
      </v-container>
    </v-card>
  </v-col>
</template>

<style lang="scss">
.navigation-container {
  position: sticky;
  position: -webkit-sticky;
  bottom: 0;
  z-index: 100;
  overflow: hidden;
}
</style>
