<script lang="ts" setup>
import NavItems from "@/layouts/components/NavItems.vue";
import Logo from "@/components/admin/Logo.vue";
import VerticalNavLayout from "@layouts/components/VerticalNavLayout.vue";

// Components
import Footer from "@/layouts/components/Footer.vue";
import NavbarThemeSwitcher from "@/layouts/components/NavbarThemeSwitcher.vue";
import NavbarLanguageSwitcher from "./NavbarLanguageSwitcher.vue";
import UserProfile from "@/layouts/components/UserProfile.vue";
</script>

<template>
  <VerticalNavLayout>
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center">
        <IconBtn
          class="ms-n3 d-lg-none"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <VIcon icon="bx-menu" />
        </IconBtn>

        <VSpacer />

        <NavbarLanguageSwitcher class="me-1" />

        <NavbarThemeSwitcher class="me-1" />

        <UserProfile />
      </div>
    </template>

    <template #vertical-nav-header="{ toggleIsOverlayNavActive }">
      <Logo class="app-title-wrapper" />

      <IconBtn
        class="d-block d-lg-none"
        @click="toggleIsOverlayNavActive(false)"
      >
        <VIcon icon="bx-x" />
      </IconBtn>
    </template>

    <template #vertical-nav-content>
      <NavItems />
    </template>

    <slot />

    <template #footer>
      <Footer />
    </template>
  </VerticalNavLayout>
</template>
