<script lang="ts" setup>
import { Lesson } from "@/models/lesson";

withDefaults(
  defineProps<{
    lesson: Lesson | null;
  }>(),
  {
    lesson: null,
  },
);
</script>

<template>
  <v-container fluid class="slideshow-container">
    <v-responsive :aspect-ratio="16 / 9" class="slide-viewer">
      <v-card class="fill-height d-flex flex-column" elevation="8">
        <transition name="slide-fade" mode="out-in">
          <div class="slide-content flex-grow-1">
            <div
              class="d-flex flex-column justify-center align-center text-center"
            >
              <h2 class="font-weight-bold text-primary">
                {{ lesson?.chapterTitle }}
              </h2>
              <h1 class="font-weight-light">
                {{ lesson?.title }}
              </h1>
            </div>
            <div class="content-html" v-html="lesson?.content"></div>

            <div v-if="lesson?.image" class="d-flex flex-column fill-height">
              <div class="d-flex align-center justify-center flex-grow-1">
                <!-- <v-img
                      :src="lesson?.image"
                      :alt="lesson?.image"
                      contain
                      max-height="90%" /> -->
              </div>
            </div>

            <p class="font-weight-light mt-8" v-if="lesson?.description">
              {{ lesson?.description }}
            </p>

            <code v-if="lesson?.notes">
              <h3 class="text-h5 mb-4">Notes</h3>
              <p class="text-body-1">{{ lesson?.notes }}</p>
            </code>
          </div>
        </transition>
      </v-card>
    </v-responsive>
  </v-container>
</template>

<style scoped>
.slideshow-container {
  background-color: #f0f2f5;
}

.slide-viewer {
  border-radius: 8px;
  overflow: hidden;
}

.slide-content {
  width: 100%;
  height: 100%;
}

.content-html {
  line-height: 1.8;
}

.content-html :deep(img) {
  max-width: 100%;
  height: auto;
  border-radius: 4px;
  margin-top: 1rem;
  margin-bottom: 1rem;
}
</style>
