<script setup>
import UiFlagDe from "@/components/shared/UiFlagDe.vue";
import UiFlagTr from "@/components/shared/UiFlagTr.vue";
import UiFlagEn from "@/components/shared/UiFlagEn.vue";
import { useLocale } from "vuetify";
import LocaleHelper from "@/utils/helpers/locale";
import config from "@/config";
const { t } = useLocale();

const languages = config.languages;

const changeLanguage = (lang) => {
  LocaleHelper.switchLanguage(lang);
};
</script>

<template>
  <div classs="pa-4">
    <perfect-scrollbar>
      <v-list class="mt-3 change-language">
        <v-list-item @click="changeLanguage(lang)" color="secondary" rounded="md" v-for="(lang, index) in languages" :key="index">
          <div class="lang-container">
            <div class="lang-icon">
              <UiFlagDe width="24" v-if="lang === 'de'" />
              <UiFlagTr width="24" v-if="lang === 'tr'" />
              <UiFlagEn width="24" v-if="lang === 'en'" />
            </div>
            <div class="lang-name">
              <v-list-item-title class="text-subtitle-2">
                {{ t(`components.verticalHeader.languages.${lang}`) }}
              </v-list-item-title>
            </div>
          </div>
        </v-list-item>
      </v-list>
    </perfect-scrollbar>
  </div>
</template>

<style lang="scss" scoped>
.lang-container {
  display: flex;
  align-items: center;
  column-gap: 0.5rem;

  .lang-icon {
    margin: 6px 0 0 0;
    padding: 0;
  }

  .lang-name {
    line-height: 1rem;
    padding: 0;
    margin: 0;
  }
}
</style>
