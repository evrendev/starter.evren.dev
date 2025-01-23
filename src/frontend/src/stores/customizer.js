import { defineStore } from "pinia";
import config from "@/config";

export const useCustomizerStore = defineStore({
  id: "customizer",
  state: () => ({
    sidebarDrawer: config.sidebarDrawer,
    customizerDrawer: config.customizerDrawer,
    miniSidebar: config.miniSidebar,
    fontTheme: config.fontTheme,
    inputBg: config.inputBg
  }),

  getters: {},
  actions: {
    SET_SIDEBAR_DRAWER() {
      this.sidebarDrawer = !this.sidebarDrawer;
    },
    SET_MINI_SIDEBAR(payload) {
      this.miniSidebar = payload;
    },
    SET_CUSTOMIZER_DRAWER(payload) {
      this.customizerDrawer = payload;
    },
    SET_FONT(payload) {
      this.fontTheme = payload;
    }
  }
});
