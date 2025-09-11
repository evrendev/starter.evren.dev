export interface LogFilters {
  sortBy: [];
  groupBy: [];
  page: number;
  itemsPerPage: number;
  startDate: string | null;
  endDate: string | null;
  search: string | null;
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
