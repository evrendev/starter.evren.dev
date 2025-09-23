export interface Filters extends AdvancedFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
}
export interface AdvancedFilters {
  search: string | null;
  startDate: string | null;
  endDate: string | null;
}

export interface SetupTwoFactorAuthenticationRequest {
  id: string;
}

export interface EnableTwoFactorAuthenticationRequest {
  id: string;
  code: string;
}

export interface DisableTwoFactorAuthenticationRequest {
  id: string;
}
