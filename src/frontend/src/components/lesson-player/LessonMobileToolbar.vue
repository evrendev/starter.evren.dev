<script setup lang="ts">
import { useTheme } from "vuetify";
import { useI18n } from "vue-i18n";
// @ts-ignore - reveal.js type definitions not available
import type Reveal from "reveal.js";

const props = defineProps<{
  revealInstance?: typeof Reveal | null;
}>();

const emit = defineEmits<{
  (e: "open-list"): void;
  (e: "open-notes"): void;
}>();

const { t } = useI18n();
const vuetifyTheme = useTheme();
const toolbarBg = computed(() => vuetifyTheme.current.value.colors.surface);
const iconColor = computed(() => vuetifyTheme.current.value.colors["on-surface"]);
const hoverBg = computed(() => vuetifyTheme.current.value.colors["on-surface"] + "14");

const prev = () => props.revealInstance?.prev();
const next = () => props.revealInstance?.next();
</script>

<template>
  <div class="mobile-toolbar">
    <button
      class="toolbar-btn"
      :aria-label="t('shared.pageList')"
      type="button"
      @click="emit('open-list')"
    >
      <v-icon icon="bx-list-ul" size="24" />
    </button>
    <button
      class="toolbar-btn"
      :aria-label="t('shared.previous')"
      type="button"
      @click="prev"
    >
      <v-icon icon="bx-chevron-left" size="24" />
    </button>
    <button
      class="toolbar-btn"
      :aria-label="t('shared.next')"
      type="button"
      @click="next"
    >
      <v-icon icon="bx-chevron-right" size="24" />
    </button>
    <button
      class="toolbar-btn"
      :aria-label="t('shared.notes')"
      type="button"
      @click="emit('open-notes')"
    >
      <v-icon icon="bx-note" size="24" />
    </button>
  </div>
</template>

<style scoped>
.mobile-toolbar {
  position: absolute;
  bottom: 16px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 10;
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 6px;
  border-radius: 999px;
  background-color: v-bind(toolbarBg);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.18);
}

.toolbar-btn {
  flex-shrink: 0;
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  border-radius: 50%;
  background: transparent;
  color: v-bind(iconColor);
  cursor: pointer;
  transition: background-color 0.2s;
}

.toolbar-btn:hover {
  background-color: v-bind(hoverBg);
}
</style>
