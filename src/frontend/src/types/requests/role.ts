export interface Filters extends BasicFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface BasicFilters {
  search: string | null;
}
