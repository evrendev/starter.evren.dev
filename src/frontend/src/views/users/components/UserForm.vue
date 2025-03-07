<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { storeToRefs } from "pinia";
import { useUserStore, useAppStore } from "@/stores";
import { PermissionsForm, InformationsForm, NavigationMenu } from "./";

const props = defineProps({
  isEdit: {
    type: Boolean,
    default: false
  }
});

const router = useRouter();
const userStore = useUserStore();
const appStore = useAppStore();

const { userInformations, userPermissions } = storeToRefs(userStore);

const userInformationsValid = ref(false);
const permissionsValid = ref(false);

const userInformationsFormRef = ref(null);
const permissionsFormRef = ref(null);

const { handleSubmit } = useForm();

const validateForms = async () => {
  userInformationsValid.value = await userInformationsFormRef.value?.validateForm();
  permissionsValid.value = await permissionsFormRef.value?.validateForm();

  if (userInformationsValid.value.valid && permissionsValid.value.valid) {
    await onSubmit();
  }
};

const onSubmit = handleSubmit(async () => {
  try {
    appStore.setLoading(true);

    const { ...informationFormValues } = userInformationsFormRef.value.values;
    const { ...permissionsFormValues } = permissionsFormRef.value.values;

    const submitData = {
      ...informationFormValues,
      ...permissionsFormValues
    };

    if (props.isEdit) {
      await userStore.update(submitData.id, submitData);
    } else {
      await userStore.create(submitData);
    }

    router.push({ name: "list-users" });
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setLoading(false);
  }
});

const handleReset = () => {
  userInformationsFormRef.value?.resetValues();
  permissionsFormRef.value?.resetValues();

  userStore.user = {};
};
</script>

<template>
  <v-row class="form-container">
    <v-col xs="12" sm="8" md="9" class="order-2 order-sm-1">
      <informations-form ref="userInformationsFormRef" :is-edit="isEdit" :user-informations="userInformations" />
      <permissions-form ref="permissionsFormRef" :is-edit="isEdit" :user-permissions="userPermissions" />
    </v-col>

    <navigation-menu :isEdit="isEdit" @reset="handleReset" @submit="validateForms" class="navigation-container order-1 order-sm-2" />
  </v-row>
</template>

<style lang="scss">
.navigation-container {
  position: sticky;
  position: -webkit-sticky;
  top: 80px;
  z-index: 100;
  height: fit-content;
}
</style>
