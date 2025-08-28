import { useAuthStore } from "@/stores/auth";
const authStore = useAuthStore();

export function hasPermission(permission: string | string[]): boolean {
  const permissions = authStore.permissions || [];

  if (Array.isArray(permission)) {
    return permission.every((perm) => permissions.includes(perm));
  }

  return permissions.includes(permission);
}
