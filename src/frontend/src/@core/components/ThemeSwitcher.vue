<script setup lang="ts">
import { useTheme } from "vuetify";
import type { ThemeSwitcherTheme } from "@layouts/types";

const props = defineProps<{
  themes: ThemeSwitcherTheme[];
}>();

const {
  name: themeName,
  global: globalTheme,
  change: changeVuetifyTheme,
} = useTheme();
const {
  state: currentThemeName,
  next: getNextThemeName,
  index: currentThemeIndex,
} = useCycleList(
  props.themes.map((t) => t.name),
  { initialValue: themeName },
);

const changeTheme = (mode: string) => {
  changeVuetifyTheme(mode);
};

// Update icon if theme is changed from other sources
watch(
  () => globalTheme.name.value,
  (val) => {
    currentThemeName.value = val;
  },
);
</script>

<template>
  <IconBtn @click="changeTheme(getNextThemeName())">
    <VIcon :icon="props.themes[currentThemeIndex].icon" />
    <VTooltip activator="parent" open-delay="1000" scroll-strategy="close">
      <span class="text-capitalize">{{ currentThemeName }}</span>
    </VTooltip>
  </IconBtn>
</template>
