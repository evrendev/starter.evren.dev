<script setup>
import { computed, defineExpose } from "vue";
import { useI18n } from "vue-i18n";
import { useForm } from "vee-validate";
import { array, object } from "yup";
import { storeToRefs } from "pinia";
import { useAuthStore } from "@/stores";

const { t } = useI18n();

const authStore = useAuthStore();
const { user } = storeToRefs(authStore);

defineProps({
  initialData: {
    type: Object,
    default: () => null
  },
  isEdit: {
    type: Boolean,
    default: false
  }
});

const defaultValues = {
  permissions: []
};

const schema = object().shape({
  permissions: array().min(1, t("admin.users.validation.permissions.required"))
});

const vuetifyConfig = (state) => ({
  props: {
    "error-messages": state.errors
  }
});

const { validate, values, errors, defineField, resetForm } = useForm({
  validationSchema: schema,
  initialValues: defaultValues
});

defineExpose({ validateForm, values, errors, resetForm });

const [permissions, permissionsProps] = defineField("permissions", vuetifyConfig);

const groupedPermissions = computed(() => {
  const modules = {};
  user.value.permissions.forEach((permission) => {
    const [module] = permission.split(".");
    if (!modules[module]) {
      modules[module] = [];
    }
    modules[module].push(permission);
  });
  return modules;
});

async function validateForm() {
  return await validate();
}
</script>

<template>
  <v-card class="mt-4">
    <v-toolbar color="secondary" dark>
      <v-toolbar-title :text="t('admin.users.helpers.permissions')" />
    </v-toolbar>

    <v-row class="pa-4 mt-2">
      <v-col cols="12">
        <div class="permissions-group">
          <div v-for="(modulePermissions, moduleName) in groupedPermissions" :key="moduleName" class="module-section mb-4">
            <div class="text-h5 mb-2 text-capitalize">{{ moduleName }}</div>
            <v-row>
              <v-col v-for="permission in modulePermissions" :key="permission" cols="auto" xs="6" class="me-sm-auto">
                <v-checkbox
                  v-model="permissions"
                  v-bind="permissionsProps"
                  :value="permission"
                  :label="permission.split('.')[1]"
                  density="comfortable"
                  color="primary"
                  hide-details
                />
              </v-col>
            </v-row>
          </div>
        </div>
      </v-col>
    </v-row>
  </v-card>
</template>

<style lang="scss" scoped>
.permissions-group {
  .module-section {
    border-bottom: 1px solid rgba(0, 0, 0, 0.12);
    padding-bottom: 1rem;

    &:last-child {
      border-bottom: none;
    }
  }
}
</style>
