<script setup lang="ts">
const route = useRoute()

const items = computed(() => {
  const matched = route.matched
    .filter((v) => v.path === route.path)
    .find((v) => !v.meta.isLayout)

  const children = matched?.children

  children
    ?.filter((c) => c.path)
    .sort(
      (a, b) =>
        ((a.meta?.drawerIndex as number) ?? 99) -
        ((b.meta?.drawerIndex as number) ?? 98),
    )
    .map((c) => ({
      title: c.meta?.title,
      to: c.name as any,
      prependIcon: c.meta?.icon,
      subtitle: c.meta?.subtitle,
    }))

  console.log(children)
  return children || []
})
</script>

<template>
  <v-container>
    <v-row>
      <v-col>
        <v-card v-for="(item, index) in items" :key="index" class="mb-1">
          <v-list-item
            v-bind="item"
            append-icon="mdi-chevron-right"
            :ripple="false"
            class="py-4"
          />
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
