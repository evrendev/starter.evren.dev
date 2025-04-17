# Store Guidelines

## Using Global Loading State

All stores should use the global loading state from the app store instead of managing their own loading state. This ensures a consistent loading experience across the application.

### How to Use the Global Loading State

1. Import the app store:

```js
import { useAppStore } from "@/stores";
```

2. For API calls that don't use the apiService directly, manage the loading state manually:

```js
async someAction() {
  const appStore = useAppStore();
  appStore.setLoading(true);

  try {
    // Your async code here
  } finally {
    appStore.setLoading(false);
  }
}
```

3. For API calls that use the apiService, the loading state is managed automatically:

```js
async someAction() {
  // apiService handles the loading state
  const response = await apiService.get("/some-endpoint");
  return response;
}
```

### Benefits

- Consistent loading experience across the application
- Centralized loading state management
- Simplified store code
- Easier to maintain and update

### Example Store

```js
import { defineStore } from "pinia";
import { apiService } from "@/utils/helpers";
import { useAppStore } from "@/stores";

export const useExampleStore = defineStore("example", {
  state: () => ({
    items: [],
    item: {},
    itemsLength: 0
  }),
  actions: {
    // For GET requests that need manual loading state management
    async getItems(searchOptions) {
      const appStore = useAppStore();
      appStore.setLoading(true);

      try {
        const params = new URLSearchParams({
          page: searchOptions.page,
          itemsPerPage: searchOptions.itemsPerPage
        });

        const response = await apiService.get(`/examples?${params}`, false);
        this.items = response.items;
        this.itemsLength = response.itemsLength;
      } finally {
        appStore.setLoading(false);
      }
    },

    // For POST/PUT/DELETE requests that use apiService
    async create(example) {
      // apiService handles the loading state
      const response = await apiService.post("/examples", example);
      return response;
    }
  }
});
```
