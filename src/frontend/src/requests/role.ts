export interface Role {
  id: string;
  name: string;
  description: string;
  isActive: boolean;
  permissions: string[];
}
export interface Filters extends BasicFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface BasicFilters {
  search: string | null;
  showActiveItems: boolean | null;
}
