<script setup lang="ts"></script>
<template>
  <v-col cols="12">
    <v-card title="Two-steps verification">
      <v-card-text>
        <p class="font-weight-semibold">
          Two factor authentication is not enabled yet.
        </p>
        <p>
          Two-factor authentication adds an additional layer of security to your
          account by requiring more than just a password to log in.
          <a href="javascript:void(0)" class="text-decoration-none"
            >Learn more.</a
          >
        </p>

        <VBtn> Enable 2FA </VBtn>
      </v-card-text>
    </v-card>
  </v-col>
</template>
