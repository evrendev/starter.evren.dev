<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useForm } from "vee-validate";
import { useUserStore, useAppStore } from "@/stores";
import { PermissionsForm, InformationsForm, ActionsButton } from "./";

const props = defineProps({
  initialData: {
    type: Object,
    default: () => null
  },
  isEdit: {
    type: Boolean,
    default: false
  }
});

const router = useRouter();
const userStore = useUserStore();
const appStore = useAppStore();

const userInformations = ref({});
const permissions = ref([]);
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
    appStore.setPageLoader(true);

    const { ...informationFormValues } = userInformationsFormRef.value.values;
    const { ...permissionsFormValues } = permissionsFormRef.value.values;

    const submitData = {
      ...informationFormValues,
      ...permissionsFormValues
    };

    if (props.isEdit) {
      await userStore.update(props.initialData.id, submitData);
    } else {
      await userStore.create(submitData);
    }

    router.push({ name: "list-users" });
  } catch (error) {
    console.error(error);
  } finally {
    appStore.setPageLoader(false);
  }
});

const handleReset = () => {
  userInformationsFormRef.value?.resetValues();
  permissionsFormRef.value?.resetValues();

  userInformations.value = {};
  permissions.value = [];
};
</script>

<template>
  <v-form>
    <v-row>
      <v-col col="12" sm="8" md="9">
        <informations-form ref="userInformationsFormRef" :is-edit="isEdit" :initial-data="initialData" />
        <permissions-form ref="permissionsFormRef" :is-edit="isEdit" :permissions="permissions" />
      </v-col>

      <actions-button :isEdit="isEdit" @reset="handleReset" @submit="validateForms" />
    </v-row>
  </v-form>
</template>
