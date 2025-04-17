import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import { manualChunksPlugin } from "vite-plugin-webpackchunkname";
import vue from "@vitejs/plugin-vue";
import svgLoader from "vite-svg-loader";
import viteImagemin from "vite-plugin-imagemin";
import vuetify from "vite-plugin-vuetify";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue({
      template: {
        compilerOptions: {
          isCustomElement: (tag) => ["v-list-recognize-title"].includes(tag)
        }
      }
    }),
    vuetify({
      autoImport: true
    }),
    manualChunksPlugin(),
    svgLoader(),
    viteImagemin({
      gifsicle: { optimizationLevel: 7 },
      optipng: { optimizationLevel: 7 },
      mozjpeg: { quality: 20 },
      pngquant: { quality: [0.6, 0.8] },
      svgo: {
        plugins: [{ name: "removeViewBox" }, { name: "cleanupIDs" }]
      }
    })
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url))
    }
  },
  server: {
    port: 5002,
    hot: true,
    watch: {
      usePolling: true
    }
  },
  css: {
    preprocessorOptions: {
      scss: {
        additionalData: `@import "@/scss/_variables.scss";`
      }
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
            return "assets/images/[name]-[hash][extname]";
          }

          if (/\.css$/.test(name ?? "")) {
            return `assets/css/${name
              .split(/\.?(?=[A-Z])/)
              .join("-")
              .toLowerCase()}-[hash][extname]`;
          }

          return "assets/[name]-[hash][extname]";
        }
      }
    },
    minify: "terser",
    terserOptions: {
      compress: {
        drop_console: true,
        drop_debugger: true
      }
    },
    sourcemap: false,
    target: "esnext"
  },
  optimizeDeps: {
    exclude: ["vuetify"],
    entries: ["./src/**/*.vue"]
  },
  base: "/"
});
