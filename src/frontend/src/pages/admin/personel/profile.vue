<script lang="ts" setup>
import { useProfileStore } from "@/stores/profile";
import { Profile, Tabs } from "@/views/admin/personel";

const useProfile = useProfileStore();
const { user, loading } = storeToRefs(useProfile);

onMounted(async () => {
  await useProfile.getUser();
});

const submit = async (values: any) => {
  await useProfile.update(values);
};
</script>

<template>
  <div>
    <tabs />

    <v-window class="mt-5 disable-tab-transition">
      <v-window-item>
        <profile :loading="loading" :user="user" @submit="submit" />
      </v-window-item>
    </v-window>
  </div>
</template>
