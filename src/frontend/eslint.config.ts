import js from "@eslint/js";
import globals from "globals";
import vue from "eslint-plugin-vue";
import tseslint from "@typescript-eslint/eslint-plugin";
import tsparser from "@typescript-eslint/parser";
import vueParser from "vue-eslint-parser";

export default [
  {
    files: ["src/**/*.{ts,tsx,vue}"],
    languageOptions: {
      ecmaVersion: 2020,
      globals: {
        ...globals.browser,
      },
      parser: vueParser,
      parserOptions: {
        ecmaVersion: "latest",
        sourceType: "module",
        extraFileExtensions: [".vue"],
        parser: tsparser,
      },
    },
    plugins: {
      "@typescript-eslint": tseslint,
      vue: vue,
    },
    rules: {
      ...(js.configs?.recommended?.rules || {}),
      ...(tseslint.configs?.recommended?.rules || {}),
      ...(vue.configs?.["vue3-recommended"]?.rules || {}),
      "no-undef": "off",
    },
  },
];
