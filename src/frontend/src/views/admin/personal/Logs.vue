<script lang="ts" setup></script>

<template>
  <v-card title="Recent Devices">
    <v-card-text>
      We need permission from your browser to show notifications.
      <a href="javascript:void(0)">Request Permission</a>
    </v-card-text>

    <v-table class="text-no-wrap">
      <thead>
        <tr>
          <th scope="col">Type</th>
          <th scope="col">EMAIL</th>
          <th scope="col">BROWSER</th>
          <th scope="col">App</th>
        </tr>
      </thead>
      <tbody></tbody>
    </v-table>
  </v-card>
</template>

<style lang="scss" scoped>
.v-table {
  th {
    text-align: start !important;
  }
}
</style>
