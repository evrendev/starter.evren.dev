export interface Filters extends BasicFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface BasicFilters {
  search: string | null;
  startDate: Date | null;
  endDate: Date | null;
  showActiveItems: boolean | null;
}

export interface UpgradeTenant {
  tenantId: string;
  extendedExpiryDate: string;
}
