import { reactive } from "vue";

// Adapts RecaptchaButton.vue's script-loading pattern (document.createElement
// script + onload flag) for the PayPal JS SDK — same idempotent "skip if
// already present" check, same shape, different script.
declare global {
  interface Window {
    paypal?: any;
  }
}

interface PayPalSdkState {
  loaded: boolean;
  error: string | null;
}

const state = reactive<PayPalSdkState>({
  loaded: false,
  error: null,
});

let loadPromise: Promise<void> | null = null;

export function usePayPalSdk() {
  const load = (): Promise<void> => {
    if (window.paypal) {
      state.loaded = true;
      return Promise.resolve();
    }

    if (loadPromise) return loadPromise;

    loadPromise = new Promise((resolve, reject) => {
      const existing = document.querySelector('script[src*="paypal.com/sdk/js"]');
      if (existing) {
        existing.addEventListener("load", () => {
          state.loaded = true;
          resolve();
        });
        return;
      }

      const clientId = import.meta.env.VITE_PAYPAL_CLIENT_ID as string;
      const currency = import.meta.env.VITE_PAYPAL_CURRENCY || "EUR";

      const script = document.createElement("script");
      script.src = `https://www.paypal.com/sdk/js?client-id=${clientId}&currency=${currency}`;
      script.async = true;

      script.onload = () => {
        state.loaded = true;
        resolve();
      };

      script.onerror = () => {
        state.error = "Failed to load PayPal SDK script.";
        reject(new Error(state.error));
      };

      document.head.appendChild(script);
    });

    return loadPromise;
  };

  return { state, load };
}
