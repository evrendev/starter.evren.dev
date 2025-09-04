<script setup lang="ts">
import { useTheme } from "vuetify";
import type { ThemeSwitcherTheme } from "@/layouts/types";

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

watch(
  () => globalTheme.name.value,
  (val) => {
    currentThemeName.value = val;
    localStorage.setItem("theme", val);
  },
);

onMounted(() => {
  const savedTheme = localStorage.getItem("theme");
  if (savedTheme && props.themes.some((t) => t.name === savedTheme)) {
    changeTheme(savedTheme);
  }
});
</script>

<template>
  <icon-btn @click="changeTheme(getNextThemeName())">
    <v-icon :icon="props.themes[currentThemeIndex].icon" />
    <v-tooltip activator="parent" open-delay="1000" scroll-strategy="close">
      <span class="text-capitalize">{{ currentThemeName }}</span>
    </v-tooltip>
  </icon-btn>
</template>
