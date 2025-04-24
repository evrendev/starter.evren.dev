<script setup>
import { shallowRef } from "vue";
import { useLocale } from "vuetify";
const { t } = useLocale();

const footerLink = shallowRef([
  {
    title: t("components.footerPanel.home"),
    url: "https://help-dunya.com/"
  },
  {
    title: t("components.footerPanel.documentation"),
    url: "https://support.help-dunya.com/documentation"
  },
  {
    title: t("components.footerPanel.support"),
    url: "https://support.help-dunya.com/"
  }
]);
</script>
<template>
  <v-footer class="px-0 footer mt-2">
    <v-row justify="center" align="center" no-gutters>
      <v-col cols="12" sm="6">
        <p class="text-body-1 mb-0 text-sm-left text-center">
          Made with
          <v-icon size="small" icon="$heart" color="error"></v-icon> in Bremen
        </p>
      </v-col>
      <v-col class="text-sm-right text-center" cols="12" sm="6">
        <a
          v-for="(item, i) in footerLink"
          :key="i"
          class="mx-2 text-body-1 text-darkText text-decoration-none"
          target="_blank"
          :href="item.url"
        >
          {{ item.title }}
        </a>
      </v-col>
    </v-row>
  </v-footer>
</template>

<style lang="scss">
.v-footer {
  position: unset;
}
footer {
  a {
    &:hover {
      color: rgb(var(--v-theme-primary)) !important;
    }
  }
}
</style>
