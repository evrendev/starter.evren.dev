<script lang="ts" setup>
import { RouteRecordNameGeneric } from "vue-router";
import { toTypedSchema } from "@vee-validate/yup";
import { array, object, string } from "yup";
import { useForm } from "vee-validate";
import {
  PERMISSION_MAP,
  ALL_UNIQUE_ACTIONS,
  type Resource,
  type Action,
} from "@/models/permission";
import { Role } from "@/models/role";

const { t } = useI18n();

const props = defineProps<{
  role: Role | null;
  pageTitle: string;
  loading: boolean;
  routeName: RouteRecordNameGeneric;
}>();

const schema = toTypedSchema(
  object({
    permissions: array()
      .of(string().required(t("admin.roles.fields.permissions.invalid")))
      .required(t("admin.roles.fields.permissions.required"))
      .min(1, t("admin.roles.fields.permissions.min")),
  }),
);

const { defineField, handleSubmit, resetForm, errors } = useForm<
  Pick<Role, "permissions">
>({
  validationSchema: schema,
});

const [permissions, permissionsAttrs] = defineField("permissions");

watch(
  () => props.role,
  (newRole) => {
    if (newRole) {
      resetForm({
        values: {
          permissions: newRole.permissions || [],
        },
      });
    } else {
      resetForm({
        values: {
          permissions: [],
        },
      });
    }
  },
  { immediate: true, deep: true },
);

const emit = defineEmits<{
  (e: "submit", values: Pick<Role, "permissions">): void;
}>();

const submit = handleSubmit((values: Pick<Role, "permissions">) => {
  emit("submit", values);
});

/**
 * Generates the standard permission string format used by the backend.
 * @param resource - The resource name (e.g., 'Users').
 * @param action - The action name (e.g., 'View').
 * @returns A formatted permission string (e.g., 'Permissions.Users.View').
 */
const generatePermissionString = (
  resource: Resource,
  action: Action,
): string => {
  // You might want to get this prefix from a config file.
  const permissionPrefix = "Permissions";
  return `${permissionPrefix}.${resource}.${action}`;
};
</script>

<template>
  <v-card elevation="6" class="mt-4" :disabled="loading">
    <v-card-title>
      <toolbar
        color="secondary"
        :title="pageTitle"
        :button="{
          icon: 'bx-chevron-left',
          text: t('shared.back'),
          to: { name: 'role-list' },
        }"
      />
    </v-card-title>
    <v-card-text>
      <v-form :disabled="loading">
        <v-row>
          <v-col v-if="errors.permissions" cols="12">
            <v-alert
              type="error"
              variant="tonal"
              density="compact"
              :text="errors.permissions"
            />
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <v-table density="compact">
              <thead>
                <tr>
                  <th class="text-left">Module</th>
                  <th
                    v-for="action in ALL_UNIQUE_ACTIONS"
                    :key="action"
                    class="text-center"
                  >
                    {{ action }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(availableActions, resource) in PERMISSION_MAP"
                  :key="resource"
                >
                  <td>
                    <span class="font-weight-bold">{{ resource }}</span>
                  </td>

                  <td
                    v-for="headerAction in ALL_UNIQUE_ACTIONS"
                    :key="headerAction"
                    class="text-center"
                  >
                    <v-checkbox
                      v-if="availableActions.includes(headerAction)"
                      v-model="permissions"
                      v-bind="permissionsAttrs"
                      density="compact"
                      hide-details
                      :value="generatePermissionString(resource, headerAction)"
                    />
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <v-btn
              color="primary"
              variant="flat"
              size="small"
              prepend-icon="bx-save"
              v-if="!loading"
              @click="submit"
            >
              {{ t("shared.save") }}
            </v-btn>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>
</template>

<style scoped type="scss">
:deep(label) {
  &.form-label {
    font-weight: 600;

    &::after {
      content: ":";
    }
  }
}

/* Add some styling for better readability */
.v-table th {
  font-weight: bold !important;
}

.v-table td,
.v-table th {
  padding: 0 8px !important;
}

.v-checkbox {
  display: inline-flex;
}
</style>
