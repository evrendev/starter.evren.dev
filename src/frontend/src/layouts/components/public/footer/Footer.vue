<script setup lang="ts">
import logo from "@/assets/images/svg/logo.svg?raw";
</script>

<template>
  <footer class="footer-wrap pt-100">
    <div class="container">
      <div class="row pb-75">
        <div class="col-xl-4 col-lg-6 col-md-6 col-sm-6">
          <div class="footer-widget">
            <router-link :to="{ name: 'home' }" class="footer-logo">
              <div v-html="logo" alt="logo" class="logo text-white" />
            </router-link>
            <p class="comp-desc">
              Lorem ipsum dolor sit amet consec tetur aditon soom isotope elit
              esumo tempo incidunt labore dolore magna aliqu core teturos
              adicing slicing elititeous sedun.
            </p>
            <ul class="social-profile list-style style1">
              <li>
                <a target="_blank" href="https://facebook.com">
                  <i class="flaticon-facebook"></i>
                </a>
              </li>
              <li>
                <a target="_blank" href="https://twitter.com">
                  <i class="flaticon-twitter"></i>
                </a>
              </li>
              <li>
                <a target="_blank" href="https://instagram.com">
                  <i class="flaticon-instagram"></i>
                </a>
              </li>
              <li>
                <a target="_blank" href="https://linkedin.com">
                  <i class="flaticon-linkedin"></i>
                </a>
              </li>
            </ul>
          </div>
        </div>
        <div class="col-xl-3 col-lg-6 col-md-6 col-sm-6 pe-xl-5">
          <div class="footer-widget">
            <h3 class="footer-widget-title">Newsletter Signup</h3>
            <form action="#" class="newsletter-form">
              <p>
                Lorem ipsum dolor, sit amet consec tetur adipi sicing elit.
                Quisquam, illum.
              </p>
              <div class="form-group">
                <input type="email" placeholder="Enter Email" />
                <button type="submit">
                  <i class="flaticon-right-arrow-1"></i>
                </button>
              </div>
            </form>
          </div>
        </div>
        <div class="col-xl-2 col-lg-6 col-md-6 col-sm-6">
          <div class="footer-widget">
            <h3 class="footer-widget-title">Our Courses</h3>
            <ul class="footer-menu list-style">
              <li>
                <a href="#">
                  <i class="flaticon-right-arrow-1"></i>
                  Learn To Drive
                </a>
              </li>
              <li>
                <a href="#">
                  <i class="flaticon-right-arrow-1"></i>
                  Course 1
                </a>
              </li>
              <li>
                <a href="course-one.html">
                  <i class="flaticon-right-arrow-1"></i>
                  Course 2
                </a>
              </li>
              <li>
                <a href="course-one.html">
                  <i class="flaticon-right-arrow-1"></i>
                  Course 3
                </a>
              </li>
              <li>
                <a href="course-one.html">
                  <i class="flaticon-right-arrow-1"></i>
                  Course 4
                </a>
              </li>
            </ul>
          </div>
        </div>
        <div class="col-xl-3 col-lg-6 col-md-6 col-sm-6">
          <div class="footer-widget">
            <h3 class="footer-widget-title">Contact Us</h3>
            <ul class="contact-info list-style">
              <li>
                <span><i class="flaticon-phone-call"></i></span>
                <h6>Call Now</h6>
                <a href="tel:02459271449">(+49) 4221 229 88 899</a>
              </li>
              <li>
                <span><i class="flaticon-envelope"></i></span>
                <h6>Email</h6>
                <a href="mailto:info@lifttech-academy.de">
                  info@lifttech-academy.de
                </a>
              </li>
              <li>
                <span><i class="flaticon-location-1"></i></span>
                <h6>Location</h6>
                <p>
                  Dittmeier - Transportgeräte Industriestraße 9 27751
                  Delmenhorst
                </p>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
    <div class="copyright-text">
      <div class="container">
        <p>
          <i class="ri-copyright-line"></i> <span>LiftTech Academy</span>. All
          Rights Reserved
        </p>
      </div>
    </div>
  </footer>
</template>
