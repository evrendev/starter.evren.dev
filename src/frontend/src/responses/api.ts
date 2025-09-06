export interface DefaultApiResponse<T> {
  succeeded: boolean;
  data?: T;
  errors?: string[];
}
export interface PaginationResponse<T> {
  page: number;
  items: T[];
  itemsPerPage: number;
  total: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  totalPages: number;
}
