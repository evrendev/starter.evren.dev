export interface Tenant {
  id: string;
  name: string;
  connectionString: string;
  adminEmail: string;
  isActive: boolean;
  validUpTo: Date;
  issuer: string;
}

export interface Filters {
  search: string | null;
  startDate: Date | null;
  endDate: Date | null;
  showActiveItems: boolean | null;
  showDeletedItems: boolean | null;
  sortBy: [];
  currentPage: number;
  itemsPerPage: number;
}
