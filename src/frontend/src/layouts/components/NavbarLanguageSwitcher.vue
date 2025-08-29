<script setup lang="ts">
import FlagTr from "@/assets/icons/FlagTr.vue";
import FlagEn from "@/assets/icons/FlagEn.vue";
import FlagDe from "@/assets/icons/FlagDe.vue";

const { t, locale } = useI18n();

import { getAvailableLanguages } from "@/utils/locale";

const languages = getAvailableLanguages();
const currentLanguage = locale;

const changeLanguageHandler = (lang: string) => {
  locale.value = lang;
  localStorage.setItem("locale", lang);
};
</script>

<template>
  <v-avatar class="cursor-pointer">
    <flag-de width="20" v-if="currentLanguage === 'de'" />
    <flag-tr width="20" v-if="currentLanguage === 'tr'" />
    <flag-en width="20" v-if="currentLanguage === 'en'" />

    <VMenu activator="parent" location="bottom end" offset="14px">
      <VList>
        <VListItem>
          <template #prepend>
            <VIcon class="me-2" icon="mdi-translate" size="22" />
          </template>

          <VListItemTitle>
            {{ t("admin.components.navbar.languages.title") }}
          </VListItemTitle>
        </VListItem>

        <VDivider class="my-2" />
        <VListItem
          v-for="lang in languages"
          :key="lang.code"
          @click="changeLanguageHandler(lang.code)"
        >
          <template #prepend>
            <flag-de width="24" v-if="lang.code === 'de'" />
            <flag-tr width="24" v-if="lang.code === 'tr'" />
            <flag-en width="24" v-if="lang.code === 'en'" />
          </template>

          <VListItemTitle>
            <span class="ml-4">
              {{ t(`admin.components.navbar.languages.${lang.code}`) }}
            </span>
          </VListItemTitle>
        </VListItem>
      </VList>
    </VMenu>
  </v-avatar>
</template>
