<script lang="ts" setup>
import { Security, Tabs } from "@/views/admin/personel";
</script>

<template>
  <div>
    <tabs />

    <v-window class="mt-5 disable-tab-transition">
      <v-window-item>
        <security />
      </v-window-item>
    </v-window>
  </div>
</template>
