export interface DefaultApiResponse<T> {
  succeeded: boolean;
  data?: T;
  errors?: string[];
}
export interface PaginationResponse<T> {
  currentPage: number;
  data: T[];
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
