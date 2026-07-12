<script lang="ts" setup>
import { LessonDetails } from "@/models/lesson";

withDefaults(
  defineProps<{
    lesson: LessonDetails | null;
  }>(),
  {
    lesson: null,
  },
);
</script>

<template>
  <v-container fluid class="pa-0 fill-height">
    <v-responsive :aspect-ratio="16 / 9" class="slide-viewer">
      <v-card>
        <v-row>
          <v-col class="text-center">
            <h4 class="font-weight-bold text-primary">
              {{ lesson?.chapter.courseTitle }} - {{ lesson?.chapter.title }}
            </h4>
            <h2 class="font-weight-light">
              {{ lesson?.title }}
            </h2>
          </v-col>
        </v-row>
        <v-row class="mt-8">
          <v-col class="text-center">
            <v-alert
              type="info"
              variant="tonal"
              class="mb-4"
            >
              {{ "Lesson content is now organized in LessonPages. View full lesson in the player." }}
            </v-alert>
            <v-btn
              :to="{ name: 'lesson-player', params: { id: lesson?.id } }"
              color="primary"
              prepend-icon="mdi-play"
            >
              Open Lesson Player
            </v-btn>
          </v-col>
        </v-row>
      </v-card>
    </v-responsive>
  </v-container>
</template>
