<script setup lang="ts">
import { onMounted, reactive } from "vue";

// Type definitions
interface RecaptchaState {
  loaded: boolean;
  error: string | null;
}

// Button variant types based on Vuetify v-btn
type ButtonVariant =
  | "flat"
  | "text"
  | "elevated"
  | "tonal"
  | "outlined"
  | "plain";

// Extend Window interface for grecaptcha
declare global {
  interface Window {
    grecaptcha: {
      execute: (
        siteKey: string,
        options: { action: string },
      ) => Promise<string>;
    };
  }
}

// Set default values for props
const props = withDefaults(
  defineProps<{
    buttonText?: string;
    buttonColor?: string;
    buttonVariant?: ButtonVariant;
    loading?: boolean;
    siteKey: string;
    action?: string;
    block?: boolean;
  }>(),
  {
    buttonText: "Submit",
    buttonColor: "primary",
    buttonVariant: "flat" as ButtonVariant,
    loading: false,
    action: "submit",
    block: false,
  },
);

// Event emit with proper typing
const emit = defineEmits<{
  "recaptcha-success": [token: string];
  "recaptcha-error": [error: Error];
}>();

// reCAPTCHA state with proper typing
const recaptcha = reactive<RecaptchaState>({
  loaded: false,
  error: null,
});

// Load reCAPTCHA script on mount
onMounted(() => {
  loadRecaptchaScript();
});

// Load reCAPTCHA script
const loadRecaptchaScript = (): void => {
  // Skip if script already loaded
  if (document.querySelector('script[src*="recaptcha/api.js"]')) {
    recaptcha.loaded = true;
    return;
  }

  const script = document.createElement("script");
  script.src = `https://www.google.com/recaptcha/api.js?render=${props.siteKey}`;
  script.async = true;
  script.defer = true;

  script.onload = () => {
    recaptcha.loaded = true;
  };

  script.onerror = (error: Event | string) => {
    console.error("reCAPTCHA script loading error:", error);
    recaptcha.error = "Failed to load reCAPTCHA script.";
    emit("recaptcha-error", new Error("Failed to load reCAPTCHA script"));
  };

  document.head.appendChild(script);
};

// Execute reCAPTCHA and get token
const executeRecaptcha = async (): Promise<string | null> => {
  if (!recaptcha.loaded || !window.grecaptcha) {
    console.error("reCAPTCHA not loaded yet");
    recaptcha.error = "reCAPTCHA not loaded yet. Please try again.";
    emit("recaptcha-error", new Error("reCAPTCHA not loaded yet"));
    return null;
  }

  try {
    const token = await window.grecaptcha.execute(props.siteKey, {
      action: props.action,
    });
    emit("recaptcha-success", token);
    return token;
  } catch (error: unknown) {
    console.error("reCAPTCHA execution error:", error);
    recaptcha.error = "Failed to execute reCAPTCHA.";
    const errorObj =
      error instanceof Error ? error : new Error("Failed to execute reCAPTCHA");
    emit("recaptcha-error", errorObj);
    return null;
  }
};

// Button click handler
const handleClick = async (): Promise<void> => {
  recaptcha.error = null;
  await executeRecaptcha();
};
</script>

<template>
  <div class="recaptcha-button-container">
    <v-btn
      type="submit"
      :block="props.block"
      :color="props.buttonColor"
      :variant="props.buttonVariant"
      :loading="props.loading"
      @click.prevent="handleClick"
    >
      {{ props.buttonText }}
    </v-btn>

    <!-- Display reCAPTCHA error if any -->
    <div v-if="recaptcha.error" class="text-error mt-2">
      {{ recaptcha.error }}
    </div>
  </div>
</template>

<style lang="scss" scoped>
.recaptcha-button-container {
  display: inline-block;
}
</style>
