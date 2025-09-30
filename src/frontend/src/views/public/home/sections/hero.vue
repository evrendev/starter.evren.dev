<script setup lang="ts"></script>

<template>
  <div class="hero-wrap bg-concrete">
    <img
      src="@/assets/images/public/home/hero-shape.png"
      alt="Image"
      class="hero-shape-one"
    />
    <div class="container">
      <div class="row gx-5 align-items-center">
        <div class="col-lg-6 col-md-11">
          <div class="hero-content">
            <span
              data-aos="fade-up"
              data-aos-duration="1200"
              data-aos-delay="200"
            >
              Lifttech Academy
            </span>
            <h1
              data-aos="fade-up"
              data-aos-duration="1200"
              data-aos-delay="300"
            >
              Lorem ipsum dolor sit amet, consectetur adipiscing elit
            </h1>
            <p data-aos="fade-up" data-aos-duration="1200" data-aos-delay="400">
              Lorem ipsum dolor sit amet, consec tetur adipi sicing elit
              temporibus repel lendus quae quaerat inventore ipsam distinctio
              officiis unde assumenda minima qui obcaecati iustor praeses antium
              ametus.
            </p>
            <a href="about.html" class="btn style1">Learn More</a>
          </div>
        </div>
        <div class="col-lg-6">
          <div class="hero-img-wrap">
            <div class="hero-shape-two">
              <img
                src="@/assets/images/public/home/hero-shape-2.png"
                alt="Image"
                class="rotate"
              />
            </div>
            <img
              class="hero-img"
              src="@/assets/images/public/home/hero-img-1.png"
              alt="Image"
            />
            <div class="hero-promo-text">
              <div class="br_1"></div>
              <div class="br_2"></div>
              <span class="semi-rotate"
                ><i class="flaticon-steering-wheel"></i
              ></span>
              <h4>5+<span>Years Experience</span></h4>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
@import "@/assets/styles/public/layout/hero";
</style>
