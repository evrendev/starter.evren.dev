import { fileURLToPath } from "node:url";
import vue from "@vitejs/plugin-vue";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { defineConfig } from "vite";
import vuetify from "vite-plugin-vuetify";
import svgLoader from "vite-svg-loader";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    // vueJsx(),

    // Docs: https://github.com/vuetifyjs/vuetify-loader/tree/master/packages/vite-plugin
    vuetify({
      styles: {
        configFile: "src/assets/styles/variables/_vuetify.scss",
      },
    }),
    Components({
      dirs: ["src/@core/components", "src/components"],
      dts: true,
      resolvers: [
        (componentName) => {
          // Auto import `VueApexCharts`
          if (componentName === "VueApexCharts")
            return {
              name: "default",
              from: "vue3-apexcharts",
              as: "VueApexCharts",
            };
        },
      ],
    }),

    // Docs: https://github.com/antfu/unplugin-auto-import#unplugin-auto-import
    AutoImport({
      imports: [
        "vue",
        "vue-router",
        "@vueuse/core",
        "@vueuse/math",
        "pinia",
        {
          "vue-i18n": ["useI18n", "createI18n"],
          "@/composables/useLocale": ["useAppLocale"],
        },
      ],
      vueTemplate: true,
      dirs: ["src/composables"],
      dts: true, // Generate auto-imports.d.ts

      // ℹ️ Disabled to avoid confusion & accidental usage
      ignore: ["useCookies", "useStorage"],
    }),
    svgLoader(),
  ],
  define: { "process.env": {} },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
      "@core": fileURLToPath(new URL("./src/@core", import.meta.url)),
      "@layouts": fileURLToPath(new URL("./src/@layouts", import.meta.url)),
      "@images": fileURLToPath(
        new URL("./src/assets/images/", import.meta.url),
      ),
      "@styles": fileURLToPath(
        new URL("./src/assets/styles/", import.meta.url),
      ),
      "@configured-variables": fileURLToPath(
        new URL(
          "./src/assets/styles/variables/_template.scss",
          import.meta.url,
        ),
      ),
    },
  },
  build: {
    chunkSizeWarningLimit: 5000,
  },
  server: {
    port: 5002,
    host: true,
  },
  optimizeDeps: {
    exclude: ["vuetify"],
    entries: ["./src/**/*.vue"],
  },
});
