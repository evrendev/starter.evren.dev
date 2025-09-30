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
          <li class="nav-item has-dropdown">
            <a href="#" class="nav-link active">
              Home
              <i class="ri-arrow-down-s-fill"></i>
            </a>
            <ul class="dropdown-menu">
              <li class="nav-item">
                <a href="index.html" class="nav-link active">Home One</a>
              </li>
              <li class="nav-item">
                <a href="index-2.html" class="nav-link">Home Two</a>
              </li>
              <li class="nav-item">
                <a href="index-3.html" class="nav-link">Home Three</a>
              </li>
            </ul>
          </li>
          <li class="nav-item">
            <a href="about.html" class="nav-link"> About Us </a>
          </li>
          <li class="nav-item has-dropdown">
            <a href="#" class="nav-link">
              Course
              <i class="ri-arrow-down-s-fill"></i>
            </a>
            <ul class="dropdown-menu">
              <li class="nav-item">
                <a href="course-one.html" class="nav-link">Course One</a>
              </li>
              <li class="nav-item">
                <a href="course-two.html" class="nav-link">Course Two</a>
              </li>
              <li class="nav-item">
                <a href="course-details.html" class="nav-link"
                  >Course Details</a
                >
              </li>
            </ul>
          </li>
          <li class="nav-item has-dropdown">
            <a href="#" class="nav-link">
              Pages
              <i class="ri-arrow-down-s-fill"></i>
            </a>
            <ul class="dropdown-menu">
              <li class="nav-item">
                <a href="#" class="nav-link">
                  Vehicles
                  <i class="ri-arrow-right-s-fill"></i>
                </a>
                <ul class="dropdown-menu">
                  <li class="nav-item">
                    <a href="vehicles.html" class="nav-link">Our Vehicles</a>
                  </li>
                  <li class="nav-item">
                    <a href="vehicle-details.html" class="nav-link"
                      >Single Vehicle</a
                    >
                  </li>
                </ul>
              </li>

              <li class="nav-item">
                <a href="instructors.html" class="nav-link">Instructor</a>
              </li>
              <li class="nav-item">
                <a href="instructor-details.html" class="nav-link"
                  >Instructor Details</a
                >
              </li>
              <li class="nav-item">
                <a href="pricing.html" class="nav-link">Pricing Plan</a>
              </li>
              <li class="nav-item">
                <a href="faq.html" class="nav-link">FAQ</a>
              </li>
              <li class="nav-item">
                <a href="testimonials.html" class="nav-link">Testimonials</a>
              </li>
              <li class="nav-item">
                <a href="#" class="nav-link">
                  User Pages
                  <i class="ri-arrow-right-s-fill"></i>
                </a>
                <ul class="dropdown-menu">
                  <li class="nav-item">
                    <a href="login.html" class="nav-link">Login</a>
                  </li>
                  <li class="nav-item">
                    <a href="register.html" class="nav-link">Register</a>
                  </li>
                  <li class="nav-item">
                    <a href="recover-password.html" class="nav-link"
                      >Recover Password</a
                    >
                  </li>
                </ul>
              </li>
              <li class="nav-item">
                <a href="terms-of-service.html" class="nav-link"
                  >Terms of Service</a
                >
              </li>
              <li class="nav-item">
                <a href="privacy-policy.html" class="nav-link"
                  >Privacy Policy</a
                >
              </li>
              <li class="nav-item">
                <a href="error-404.html" class="nav-link">404 Error Page</a>
              </li>
            </ul>
          </li>
          <li class="nav-item has-dropdown">
            <a href="#" class="nav-link">
              Blog
              <i class="ri-arrow-down-s-fill"></i>
            </a>
            <ul class="dropdown-menu">
              <li class="nav-item">
                <a href="#" class="nav-link">
                  Blog Layout
                  <i class="ri-arrow-right-s-fill"></i>
                </a>
                <ul class="dropdown-menu">
                  <li class="nav-item">
                    <a href="blog-no-sidebar.html" class="nav-link"
                      >Blog Grid</a
                    >
                  </li>
                  <li class="nav-item">
                    <a href="blog-left-sidebar.html" class="nav-link"
                      >Blog Left Sidebar</a
                    >
                  </li>
                  <li class="nav-item">
                    <a href="blog-right-sidebar.html" class="nav-link"
                      >Blog Right Sidebar</a
                    >
                  </li>
                </ul>
              </li>
              <li class="nav-item">
                <a href="#" class="nav-link">
                  Single Blog
                  <i class="ri-arrow-right-s-fill"></i>
                </a>
                <ul class="dropdown-menu">
                  <li class="nav-item">
                    <a href="blog-details-no-sidebar.html" class="nav-link"
                      >Blog Details No Sidebar</a
                    >
                  </li>
                  <li class="nav-item">
                    <a href="blog-details-left-sidebar.html" class="nav-link"
                      >Blog Details Left Sidebar</a
                    >
                  </li>
                  <li class="nav-item">
                    <a href="blog-details-right-sidebar.html" class="nav-link"
                      >Blog Details Right Sidebar</a
                    >
                  </li>
                </ul>
              </li>
            </ul>
          </li>
          <li class="nav-item">
            <a href="contact.html" class="nav-link">Contact Us</a>
          </li>
          <li class="nav-item xl-none">
            <a href="course-one.html" class="btn">Get Started</a>
          </li>
        </ul>
        <div class="others-options lg-none">
          <div class="option-item">
            <button class="searchbtn" type="button">
              <i class="flaticon-search"></i>
            </button>
          </div>
          <div class="header-btn">
            <a href="course-one.html" class="btn">Get Started</a>
          </div>
        </div>
      </div>
    </nav>
  </div>
</template>
