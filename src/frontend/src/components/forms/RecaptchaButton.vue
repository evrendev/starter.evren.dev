<script setup>
import { onMounted, reactive } from "vue";

const props = defineProps({
  buttonText: {
    type: String,
    default: "Submit"
  },
  buttonColor: {
    type: String,
    default: "primary"
  },
  buttonVariant: {
    type: String,
    default: "flat"
  },
  buttonIcon: {
    type: String,
    default: ""
  },
  loading: {
    type: Boolean,
    default: false
  },
  siteKey: {
    type: String,
    required: true
  },
  action: {
    type: String,
    default: "submit"
  }
});

// Event emit
const emit = defineEmits(["recaptcha-success", "recaptcha-error"]);

// reCAPTCHA state
const recaptcha = reactive({
  loaded: false,
  error: null
});

// Load reCAPTCHA script on mount
onMounted(() => {
  loadRecaptchaScript();
});

// Load reCAPTCHA script
const loadRecaptchaScript = () => {
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

  script.onerror = (error) => {
    console.error("reCAPTCHA script loading error:", error);
    recaptcha.error = "Failed to load reCAPTCHA script.";
    emit("recaptcha-error", new Error("Failed to load reCAPTCHA script"));
  };

  document.head.appendChild(script);
};

// Execute reCAPTCHA and get token
const executeRecaptcha = async () => {
  if (!recaptcha.loaded || !window.grecaptcha) {
    console.error("reCAPTCHA not loaded yet");
    recaptcha.error = "reCAPTCHA not loaded yet. Please try again.";
    emit("recaptcha-error", new Error("reCAPTCHA not loaded yet"));
    return null;
  }

  try {
    const token = await window.grecaptcha.execute(props.siteKey, { action: props.action });
    emit("recaptcha-success", token);
    return token;
  } catch (error) {
    console.error("reCAPTCHA execution error:", error);
    recaptcha.error = "Failed to execute reCAPTCHA.";
    emit("recaptcha-error", error);
    return null;
  }
};

// Button click handler
const handleClick = async () => {
  recaptcha.error = null;
  await executeRecaptcha();
};
</script>

<template>
  <div class="recaptcha-button-container">
    <v-btn
      :color="buttonColor"
      :variant="buttonVariant"
      type="submit"
      :loading="loading"
      @click.prevent="handleClick"
      :prepend-icon="buttonIcon"
    >
      {{ buttonText }}
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
