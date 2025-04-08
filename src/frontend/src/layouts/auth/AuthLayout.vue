<script setup>
import { RouterView } from "vue-router";
import { useCustomizerStore } from "@/stores";
const customizer = useCustomizerStore();
</script>

<template>
  <v-app
    :theme="customizer.theme"
    :class="[customizer.fontTheme, customizer.miniSidebar ? 'mini-sidebar' : '', customizer.inputBg ? 'inputWithbg' : '']"
  >
    <v-row class="h-screen" no-gutters>
      <v-col cols="12" class="d-flex align-center">
        <v-container>
          <div class="pa-7 pa-sm-12">
            <v-row justify="center">
              <v-col cols="12" lg="10" xl="6" md="7">
                <v-card elevation="0" class="login-box">
                  <v-card variant="outlined">
                    <v-card-text class="pa-lg-9 pa-sm-2">
                      <router-view />
                    </v-card-text>
                  </v-card>
                </v-card>
              </v-col>
            </v-row>
          </div>
        </v-container>
      </v-col>
    </v-row>
  </v-app>
</template>

<style lang="scss" scoped>
.login-box {
  max-width: 475px;
  margin: 0 auto;
}
</style>
