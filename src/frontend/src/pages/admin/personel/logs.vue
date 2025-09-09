<script lang="ts" setup>
import { Logs, Tabs } from "@/views/admin/personel";
</script>

<template>
  <div>
    <tabs />

    <v-window class="mt-5 disable-tab-transition">
      <v-window-item>
        <logs />
      </v-window-item>
    </v-window>
  </div>
</template>
