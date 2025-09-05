<script setup lang="ts">
import { EnglishFlag, DeutschFlag, TurkishFlag } from "@/assets/icons";

const { t, locale } = useI18n();

import { getAvailableLanguages } from "@/utils/locale";

const languages = getAvailableLanguages();

const changeLanguageHandler = (lang: string) => {
  locale.value = lang;
  localStorage.setItem("locale", lang);
};
</script>

<template>
  <icon-btn>
    <VIcon>
      <deutsch-flag v-if="locale === 'de'" />
      <turkish-flag v-if="locale === 'tr'" />
      <english-flag v-if="locale === 'en'" />
    </VIcon>
    <VTooltip activator="parent" open-delay="1000" scroll-strategy="close">
      <span class="text-capitalize">
        {{ t(`admin.components.navbar.languages.${locale}`) }}
      </span>
    </VTooltip>

    <v-menu activator="parent" location="bottom end" offset="14px">
      <v-list>
        <v-list-item>
          <template #prepend>
            <v-icon class="me-2" icon="mdi-translate" size="22" />
          </template>

          <v-list-item-title>
            {{ t("admin.components.navbar.languages.title") }}
          </v-list-item-title>
        </v-list-item>

        <v-divider class="my-2" />

        <v-list-item
          v-for="lang in languages.filter(
            (language: { code: string }) => language.code !== locale,
          )"
          :key="lang.code"
          @click="changeLanguageHandler(lang.code)"
        >
          <template #prepend>
            <deutsch-flag width="24" v-if="lang.code === 'de'" />
            <turkish-flag width="24" v-if="lang.code === 'tr'" />
            <english-flag width="24" v-if="lang.code === 'en'" />
          </template>

          <v-list-item-title>
            <span class="ml-4">
              {{ t(`admin.components.navbar.languages.${lang.code}`) }}
            </span>
          </v-list-item-title>
        </v-list-item>
      </v-list>
    </v-menu>
  </icon-btn>
</template>
