---
description: "Guidelines for modern frontend development using TypeScript 5.x, ES2022, and Vue 3 Composition API with best practices for maintainability and performance."
applyTo: "**/*.ts, **/*.tsx, **/*.vue, **/*.js, **/*.jsx, **/*.scss"
---

# Frontend Development Guidelines (TypeScript & Vue 3)

## Core Intent & General Standards

- **TypeScript Target:** Target **TypeScript 5.x** and compile to **ES2022** JavaScript baseline.
- **Architecture:** Respect the existing project architecture and coding standards. Extend current abstractions before inventing new ones.
- **Clarity & Maintainability:** Prioritize **readable, explicit solutions** over clever shortcuts. Keep methods, composables, and components short and focused (Single Responsibility Principle).
- **Modularity:** Use pure **ES modules**. Never emit `require` or CommonJS helpers.
- **Tooling:** Rely on the project's build, lint, and test scripts. Match the project's indentation, quote style, and trailing comma rules.

---

## TypeScript Expectations

- **Type Safety:** Enable and adhere to **`strict` mode** in `tsconfig.json`. Avoid **`any`** (implicit or explicit); prefer **`unknown`** plus narrowing.
- **Type Design:** Use **discriminated unions** for complex state or events. Use interfaces or type aliases for complex data shapes.
- **Utility Types:** Express intent with built-in TypeScript utility types (e.g., `Readonly`, `Partial`, `Record`).
- **Naming:** Use **PascalCase** for classes, interfaces, enums, and type aliases. Use **camelCase** for everything else. **Do not use interface prefixes** like `I`.
- **`nameof`:** Name variables and members for their behavior or domain meaning, not their implementation.

---

## Vue 3 & Component Design

- **API Preference:** Favor the **Composition API** (`<script setup>`) over the Options API.
- **State Management:** Use **Pinia** for global application state. For component-local state, use `ref` or `reactive`. Use `computed` for derived state.
- **Composables:** Extract reusable logic into dedicated **composable functions** (e.g., `useFetch`, `useAuth`) in a `composables/` directory.
- **Component Naming:** Use **PascalCase** for component names (e.g., `UserProfileCard`) and **kebab-case** for file names (e.g., `user-profile-card.vue`).
- **Props & Emits:** Use `defineProps` and `defineEmits` with explicit types.
- **Styling:** Use **`<style scoped>`** for component-level styles. Implement mobile-first, responsive design.

---

## Async, Data, and Error Handling

- **Asynchronous Logic:** Use **`async/await`**. For data fetching, use composables (like Vue Query) and explicitly handle **loading, error, and success states**.
- **Error Handling:** Wrap awaits/risky logic in **`try/catch`**. Use the global error handler (`app.config.errorHandler`) for uncaught errors. Log errors via the project's telemetry utilities.
- **Cancellation:** Apply retries, backoff, and **cancellation** to network/I/O calls, especially on component unmount (`onUnmounted`) or dependency changes.
- **Normalization:** Normalize external API responses and map external errors to domain-specific shapes.

---

## Testing Expectations

- **Test Style:** Add or update **unit tests** (using Vue Test Utils and Jest/Vitest) focusing on behavior, not internal implementation details.
- **Scope:** Use `mount` and **`shallowMount`** for component isolation.
- **Mocks:** Mock global plugins (router, Pinia) and external dependencies as needed for unit testing.
- **E2E/Integration:** Expand integration or end-to-end suites (Cypress/Playwright) when behavior crosses modules.

---

## Security and Performance

- **Security:**
  - **Avoid `v-html`**. Sanitize all external or user-provided HTML inputs rigorously.
  - Validate and sanitize external input using schema validators or type guards.
  - **Never hardcode secrets**. Load them from secure sources.
- **Performance:**
  - **Lazy-load components** using dynamic imports (`defineAsyncComponent`).
  - **Defer expensive work** until users need it.
  - **Batch or debounce** high-frequency events (e.g., input validation, config updates).
  - Use `v-once` or `v-memo` for static or infrequently changing elements.
  - Cleanup side effects in `onUnmounted` or `watch` cleanup callbacks to prevent **memory leaks**.

---

## Documentation

- **JSDoc/TSDoc:** Add **JSDoc/TSDoc** to all public APIs, components, and composables; include `@remarks` or `@example` when helpful.
- **Comments:** Write comments that capture **intent** (the _why_), not just the _what_. Remove stale notes during refactors.
