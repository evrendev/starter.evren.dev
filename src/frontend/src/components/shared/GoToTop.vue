<script setup>
import { ref, onMounted, onUnmounted } from "vue";

const showButton = ref(false);
const scrollThreshold = 300; // Show button after scrolling 300px

const checkScroll = () => {
  showButton.value = window.scrollY > scrollThreshold;
};

const scrollToTop = () => {
  window.scrollTo({
    top: 0,
    behavior: "smooth"
  });
};

onMounted(() => {
  window.addEventListener("scroll", checkScroll);
});

onUnmounted(() => {
  window.removeEventListener("scroll", checkScroll);
});
</script>

<template>
  <v-fade-transition>
    <v-btn v-show="showButton" icon="$chevronUp" color="primary" class="go-to-top-btn" @click="scrollToTop" />
  </v-fade-transition>
</template>

<style lang="scss" scoped>
.go-to-top-btn {
  position: fixed;
  bottom: 20px;
  right: 20px;
  z-index: 1001;
}
</style>
