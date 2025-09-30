<script setup lang="ts">
import logo from "@/assets/images/svg/logo.svg?raw";
</script>

<template>
  <div class="header-bottom">
    <nav class="navbar navbar-expand-md navbar-light">
      <router-link :to="{ name: 'home' }" class="navbar-brand">
        <div v-html="logo" alt="logo" class="logo text-white" />
      </router-link>
      <div
        class="collapse navbar-collapse main-menu-wrap"
        id="navbarSupportedContent"
      >
        <div class="menu-close xl-none">
          <a href="javascript:void(0)"> <i class="ri-close-line"></i></a>
        </div>
        <ul class="navbar-nav ms-auto">
          <li class="nav-item">
            <a href="#" class="nav-link active"> Home </a>
          </li>
          <li class="nav-item">
            <a href="#" class="nav-link"> About Us </a>
          </li>
          <li class="nav-item has-dropdown">
            <a href="#" class="nav-link">
              Course
              <i class="ri-arrow-down-s-fill"></i>
            </a>
            <ul class="dropdown-menu">
              <li class="nav-item">
                <a href="#" class="nav-link">Course One</a>
              </li>
              <li class="nav-item">
                <a href="#" class="nav-link">Course Two</a>
              </li>
              <li class="nav-item">
                <a href="#" class="nav-link">Course Three</a>
              </li>
            </ul>
          </li>
          <li class="nav-item">
            <a href="#" class="nav-link"> About Us </a>
          </li>
          <li class="nav-item">
            <a href="#" class="nav-link">Contact Us</a>
          </li>
          <li class="nav-item xl-none">
            <a href="#" class="btn">Register / Login</a>
          </li>
        </ul>
        <div class="others-options lg-none">
          <div class="option-item">
            <button class="searchbtn" type="button">
              <i class="flaticon-search"></i>
            </button>
          </div>
          <div class="header-btn">
            <a href="#" class="btn">Register / Login</a>
          </div>
        </div>
      </div>
    </nav>
  </div>
</template>
