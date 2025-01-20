<script setup>
import { LogIn, ForgotPassword, TwoFactorAuthentication } from "./forms";
import { ref, watch } from "vue";
import { useRoute } from "vue-router";
const route = useRoute();

let page = ref(route.params.page);

watch(
  () => route.params.page,
  (newPage, oldPage) => {
    page.value = newPage;
  }
);
</script>

<template>
  <v-row class="h-screen" no-gutters>
    <v-col cols="12" class="d-flex align-center bg-lightprimary">
      <v-container>
        <div class="pa-7 pa-sm-12">
          <v-row justify="center">
            <v-col cols="12" lg="10" xl="6" md="7">
              <v-card elevation="0" class="login-box">
                <v-card variant="outlined">
                  <v-card-text class="pa-lg-9 pa-sm-2">
                    <forgot-password v-if="page === 'forgot-password'" />

                    <two-factor-authentication v-else-if="page === '2fa'" />

                    <log-in v-else />
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
.login-box {
  max-width: 475px;
  margin: 0 auto;
}
</style>
