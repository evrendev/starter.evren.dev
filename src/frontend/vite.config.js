import { fileURLToPath, URL } from "node:url"
import { resolve, dirname } from "path"
import { defineConfig } from "vite"
import { manualChunksPlugin } from "vite-plugin-webpackchunkname"
import vue from "@vitejs/plugin-vue"
import svgLoader from "vite-svg-loader"
import VueI18nPlugin from "@intlify/unplugin-vue-i18n/vite"
import vueDevTools from "vite-plugin-vue-devtools"
import viteImagemin from "vite-plugin-imagemin"
import vuetify from 'vite-plugin-vuetify';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue({
      template: {
        compilerOptions: {
          isCustomElement: (tag) => ['v-list-recognize-title'].includes(tag)
        }
      }
    }),
    vuetify({
      autoImport: true
    }),
    vueDevTools(),
    VueI18nPlugin({
      runtimeOnly: false,
      include: resolve(
        dirname(fileURLToPath(import.meta.url)),
        "./src/plugins/i18n/locales/**",
      ),
    }),
    manualChunksPlugin(),
    svgLoader(),
    viteImagemin({
      gifsicle: { optimizationLevel: 7 },
      optipng: { optimizationLevel: 7 },
      mozjpeg: { quality: 20 },
      pngquant: { quality: [0.6, 0.8] },
      svgo: {
        plugins: [{ name: "removeViewBox" }, { name: "cleanupIDs" }],
      },
    }),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  server: {
    port: 5002,
    hot: true,
  },
  css: {
    preprocessorOptions: {
      scss: {}
    }
  },
  build: {
    chunkSizeWarningLimit: 1024 * 1024,
    rollupOptions: {
      output: {
        chunkFileNames: "static/chunks/[name]-[hash].js",
        entryFileNames: "static/entries/[name]-[hash].js",
        assetFileNames: ({ name }) => {
          if (/\.(gif|jpe?g|png|svg)$/.test(name ?? "")) {
            return "assets/images/[name]-[hash][extname]"
          }

          if (/\.css$/.test(name ?? "")) {
            return `assets/css/${name
              .split(/\.?(?=[A-Z])/)
              .join("-")
              .toLowerCase()}-[hash][extname]`
          }

          return "assets/[name]-[hash][extname]"
        },
      },
    },
    minify: "terser",
    terserOptions: {
      compress: {
        drop_console: true,
        drop_debugger: true,
      },
    },
    sourcemap: false,
    target: "esnext",
  },
  optimizeDeps: {
    exclude: ['vuetify'],
    entries: ['./src/**/*.vue']
  },
  base: "/",
});
