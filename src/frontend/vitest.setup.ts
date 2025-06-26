import { config } from "@vue/test-utils";
import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import { createTestingPinia } from "@pinia/testing";
import { vi } from "vitest";

const vuetify = createVuetify({
  components,
  directives,
});

config.global.plugins = [
  vuetify,
  createTestingPinia({
    createSpy: vi.fn,
  }),
];

config.global.stubs = {
  $t: (key: string) => key,
};

config.global.mocks = {
  $router: {
    push: vi.fn(),
  },
  $route: {
    params: {},
    query: {},
  },
};
