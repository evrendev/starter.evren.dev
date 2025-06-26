import { fileURLToPath, URL } from "node:url";

import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import vuetify from "vite-plugin-vuetify"; // Vuetify eklentisini import edin

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vuetify({
      autoImport: true, // Vuetify bileşenlerini otomatik olarak import et
      styles: {
        configFile: "src/assets/styles/settings.scss", // Vuetify stil ayarları için dosya (isteğe bağlı)
      },
    }),
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  // Vitest yapılandırması
  test: {
    globals: true, // `test`, `expect` gibi fonksiyonları global yapar
    environment: "jsdom", // Tarayıcı ortamını simüle eder
    setupFiles: "./vitest.setup.ts", // Her testten önce çalışacak kurulum dosyası
    coverage: {
      reporter: ["text", "json", "html"],
      reportsDirectory: "./coverage",
    },
  },
});
