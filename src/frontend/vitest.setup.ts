import { config } from "@vue/test-utils";
import { createVuetify } from "vuetify";
import * as components from "vuetify/components";
import * as directives from "vuetify/directives";
import { createTestingPinia } from "@pinia/testing";
import { vi } from "vitest"; // Vitest'in kendi mock fonksiyonu

// Vuetify kurulumu
const vuetify = createVuetify({
  components,
  directives,
});

// Vue Test Utils global config
config.global.plugins = [
  vuetify,
  // createTestingPinia Pinia store'larını testlerde izlemenize veya mock'lamanıza olanak tanır
  createTestingPinia({
    createSpy: vi.fn, // Vitest'in kendi mock fonksiyonunu kullan
  }),
];

// i18n mock'u - testlerde çeviri anahtarlarını doğrudan döndürür
config.global.stubs = {
  $t: (key: string) => key,
};

// Router mock'u (örneğin useRouter kullanan bileşenler için)
config.global.mocks = {
  $router: {
    push: vi.fn(),
    // Diğer router metotları (opsiyonel)
  },
  $route: {
    params: {},
    query: {},
    // Diğer route özellikleri (opsiyonel)
  },
};

// Diğer global mock'lar veya kurulumlar buraya eklenebilir
