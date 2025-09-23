export interface Filters extends AdvancedFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface AdvancedFilters {
  search: string | null;
  startDate: Date | null;
  endDate: Date | null;
  showActiveItems: boolean | null;
}

export interface UpgradeTenant {
  tenantId: string;
  extendedExpiryDate: string;
}
