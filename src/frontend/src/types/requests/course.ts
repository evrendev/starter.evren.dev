export interface Filters extends AdvancedFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface AdvancedFilters {
  search: string | null;
  categoryId: string | null;
  published: boolean | null;
}
