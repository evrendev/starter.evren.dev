<script setup>
import Logo from "@/layouts/full/logo/LogoDark.vue"
import AuthLogin from "./authForms/AuthLogin.vue"
import { useLocale } from "vuetify"

const { t } = useLocale()
</script>

<template>
  <v-row class="h-screen" no-gutters>
    <v-col cols="12" class="d-flex align-center bg-lightprimary">
      <v-container>
        <div class="pa-7 pa-sm-12">
          <v-row justify="center">
            <v-col cols="12" lg="10" xl="6" md="7">
              <v-card elevation="0" class="loginBox">
                <v-card variant="outlined">
                  <v-card-text class="pa-9">
                    <v-row>
                      <v-col cols="12" class="text-center">
                        <Logo />
                        <h2 class="text-secondary text-h2 mt-8">
                          {{ t("auth.login.welcome") }}
                        </h2>
                        <h4 class="text-disabled text-h4 mt-3">
                          {{ t("auth.login.subtitle") }}
                        </h4>
                      </v-col>
                    </v-row>
                    <AuthLogin />
                  </v-card-text>
                </v-card>
              </v-card>
            </v-col>
          </v-row>
        </div>
      </v-container>
    </v-col>
  </v-row>
</template>
<style lang="scss">
.loginBox {
  max-width: 475px;
  margin: 0 auto;
}
</style>
