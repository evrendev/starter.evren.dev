<script setup>
import { computed, defineExpose, ref } from "vue";
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

const { validate, values, defineField, resetForm } = useForm({
  validationSchema: schema,
  initialValues: defaultValues
});

defineExpose({ validateForm, values, resetValues });

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

const moduleSelectionState = computed(() => {
  const state = {};
  Object.entries(groupedPermissions.value).forEach(([moduleName, modulePermissions]) => {
    const selectedCount = modulePermissions.filter((permission) => permissions.value.includes(permission)).length;
    state[moduleName] = {
      selected: selectedCount === modulePermissions.length,
      indeterminate: selectedCount > 0 && selectedCount < modulePermissions.length
    };
  });
  return state;
});

function toggleModulePermissions(moduleName) {
  const modulePermissions = groupedPermissions.value[moduleName];
  if (moduleSelectionState.value[moduleName].selected) {
    // Deselect all permissions in the module
    permissions.value = permissions.value.filter((p) => !modulePermissions.includes(p));
  } else {
    // Select all permissions in the module
    const newPermissions = new Set([...permissions.value, ...modulePermissions]);
    permissions.value = Array.from(newPermissions);
  }
}

const isValid = ref(false);

async function validateForm() {
  isValid.value = await validate();

  return isValid.value;
}

function resetValues() {
  resetForm();
  isValid.value = false;
}
</script>

<template>
  <v-card class="mt-4">
    <v-toolbar color="primary" id="user-permissions">
      <v-toolbar-title :text="t('admin.users.helpers.permissions')" />
      <v-btn icon>
        <v-icon icon="$shieldAccount" />
      </v-btn>
    </v-toolbar>

    <v-row class="pa-4" v-show="isValid">
      <v-col>
        <v-alert density="compact" :title="t('common.error')" type="error" :text="t('admin.users.validation.permissions.required')" />
      </v-col>
    </v-row>

    <v-row class="pa-4 mt-1" v-for="(modulePermissions, moduleName) in groupedPermissions" :key="moduleName">
      <v-col cols="12">
        <v-card class="mx-auto" border flat>
          <v-list-item class="px-6">
            <template v-slot:prepend>
              <v-avatar color="surface-light" size="32">🎯</v-avatar>
            </template>

            <template v-slot:title> {{ moduleName }} </template>

            <template v-slot:append>
              <v-switch
                class="ms-4"
                :model-value="moduleSelectionState[moduleName].selected"
                :indeterminate="moduleSelectionState[moduleName].indeterminate"
                @update:model-value="toggleModulePermissions(moduleName)"
                :label="t('common.selectAll')"
                density="comfortable"
                color="primary"
                hide-details
              />
            </template>
          </v-list-item>

          <v-divider></v-divider>

          <v-card-text class="text-medium-emphasis pa-6">
            <div class="d-flex align-center justify-space-between">
              <v-col v-for="permission in modulePermissions" :key="permission" cols="auto" xs="6" class="me-sm-auto">
                <v-switch
                  v-model="permissions"
                  v-bind="permissionsProps"
                  :value="permission"
                  :label="permission.split('.')[1]"
                  density="comfortable"
                  color="primary"
                  hide-details
                />
              </v-col>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-card>
</template>
