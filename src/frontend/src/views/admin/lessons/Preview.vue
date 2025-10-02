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
        <v-row>
          <v-col :cols="lesson?.imageUrl ? 8 : 12">
            <div class="content-html" v-html="lesson?.content"></div>
          </v-col>
          <v-col
            v-if="lesson?.imageUrl"
            cols="4"
            class="d-flex align-center bg-grey-lighten-4"
          >
            <v-img :src="lesson?.imageUrl ?? 'empty.png'" />
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <p
              class="font-weight-light font-italic text-subtitle-2 mt-8"
              v-if="lesson?.description"
            >
              {{ lesson?.description }}
            </p>

            <blockquote
              v-if="lesson?.notes"
              class="bg-yellow-lighten-4 border-b-sm pa-3 rounded-lg"
            >
              <h6 class="text-h6 mb-2">Notes</h6>
              <p>{{ lesson?.notes }}</p>
            </blockquote>
          </v-col>
        </v-row>
      </v-card>
    </v-responsive>
  </v-container>
</template>
