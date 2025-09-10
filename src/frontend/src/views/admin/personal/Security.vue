<script lang="ts" setup>
const isCurrentPasswordVisible = ref(false);
const isNewPasswordVisible = ref(false);
const isConfirmPasswordVisible = ref(false);
const currentPassword = ref("12345678");
const newPassword = ref("87654321");
const confirmPassword = ref("87654321");

const passwordRequirements = [
  "Minimum 8 characters long - the more, the better",
  "At least one lowercase character",
  "At least one number, symbol, or whitespace character",
];
</script>

<template>
  <v-row>
    <v-col cols="12">
      <v-card title="Change Password">
        <VForm>
          <v-card-text>
            <!-- 👉 Current Password -->
            <v-row>
              <v-col cols="12" md="6">
                <!-- 👉 current password -->
                <v-text-field
                  v-model="currentPassword"
                  :type="isCurrentPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="
                    isCurrentPasswordVisible ? 'bx-hide' : 'bx-show'
                  "
                  label="Current Password"
                  placeholder="············"
                  @click:append-inner="
                    isCurrentPasswordVisible = !isCurrentPasswordVisible
                  "
                />
              </v-col>
            </v-row>

            <!-- 👉 New Password -->
            <v-row>
              <v-col cols="12" md="6">
                <!-- 👉 new password -->
                <v-text-field
                  v-model="newPassword"
                  :type="isNewPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="
                    isNewPasswordVisible ? 'bx-hide' : 'bx-show'
                  "
                  label="New Password"
                  autocomplete="on"
                  placeholder="············"
                  @click:append-inner="
                    isNewPasswordVisible = !isNewPasswordVisible
                  "
                />
              </v-col>

              <v-col cols="12" md="6">
                <!-- 👉 confirm password -->
                <v-text-field
                  v-model="confirmPassword"
                  :type="isConfirmPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="
                    isConfirmPasswordVisible ? 'bx-hide' : 'bx-show'
                  "
                  label="Confirm New Password"
                  placeholder="············"
                  @click:append-inner="
                    isConfirmPasswordVisible = !isConfirmPasswordVisible
                  "
                />
              </v-col>
            </v-row>
          </v-card-text>

          <!-- 👉 Password Requirements -->
          <v-card-text>
            <p class="text-base font-weight-medium mt-2">
              Password Requirements:
            </p>

            <ul class="d-flex flex-column gap-y-3">
              <li
                v-for="item in passwordRequirements"
                :key="item"
                class="d-flex"
              >
                <div>
                  <VIcon size="7" icon="bxs-circle" class="me-3" />
                </div>
                <span class="font-weight-medium">{{ item }}</span>
              </li>
            </ul>
          </v-card-text>

          <!-- 👉 Action Buttons -->
          <v-card-text class="d-flex flex-wrap gap-4">
            <VBtn>Save changes</VBtn>

            <VBtn type="reset" color="secondary" variant="tonal"> Reset </VBtn>
          </v-card-text>
        </VForm>
      </v-card>
    </v-col>
    <v-col cols="12">
      <v-card title="Two-steps verification">
        <v-card-text>
          <p class="font-weight-semibold">
            Two factor authentication is not enabled yet.
          </p>
          <p>
            Two-factor authentication adds an additional layer of security to
            your account by requiring more than just a password to log in.
            <a href="javascript:void(0)" class="text-decoration-none"
              >Learn more.</a
            >
          </p>

          <VBtn> Enable 2FA </VBtn>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>
