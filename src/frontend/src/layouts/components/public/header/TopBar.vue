<script setup lang="ts">
import { EnglishFlag, DeutschFlag, TurkishFlag } from "@/assets/icons";
import CloseSidebar from "@/components/public/CloseSidebar.vue";
</script>

<template>
  <div class="header-top">
    <close-sidebar />

    <div class="container">
      <div class="row align-items-center">
        <div class="col-lg-8 col-md-12">
          <div class="header-top-left">
            <ul class="contact-info list-style">
              <li>
                <i class="flaticon-call"></i>
                <a href="tel:02459271449">(+49) 4221 229 88 899</a>
              </li>
              <li>
                <i class="flaticon-envelope"></i>
                <a href="mailto:info@lifttech-academy.de">
                  info@lifttech-academy.de
                </a>
              </li>
              <li>
                <i class="flaticon-location-1"></i>
                <p>
                  Dittmeier - Transportgeräte Industriestraße 9 27751
                  Delmenhorst
                </p>
              </li>
            </ul>
          </div>
        </div>
        <div class="col-lg-4 col-md-12">
          <div class="header-top-right">
            <ul class="social-profile list-style">
              <li>
                <a target="_blank" href="https://facebook.com">
                  <i class="ri-facebook-fill"></i>
                </a>
              </li>
              <li>
                <a target="_blank" href="https://linkedin.com">
                  <i class="ri-linkedin-fill"></i>
                </a>
              </li>
              <li>
                <a target="_blank" href="https://twitter.com">
                  <i class="ri-twitter-fill"></i>
                </a>
              </li>
            </ul>
            <div class="select-lang">
              <i class="ri-global-line"></i>
              <div
                class="navbar-option-item navbar-language dropdown language-option"
              >
                <button
                  class="dropdown-toggle"
                  type="button"
                  data-bs-toggle="dropdown"
                  aria-haspopup="true"
                  aria-expanded="false"
                >
                  <span class="lang-name"> Deutsch </span>
                </button>
                <div class="dropdown-menu language-dropdown-menu">
                  <a class="dropdown-item" href="#">
                    <english-flag width="24" />
                    English
                  </a>
                  <a class="dropdown-item" href="#">
                    <turkish-flag width="24" />
                    Türkçe
                  </a>
                  <a class="dropdown-item" href="#">
                    <deutsch-flag width="24" />
                    Deutsch
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
