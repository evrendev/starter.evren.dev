export interface ApiErrorResponse {
  messages: string[];
  source?: string;
  exception?: string;
  errorId: string;
  supportMessage?: string;
  statusCode: number;
}

export interface ApiResponse<T> {
  succeeded: boolean;
  data: T;
  errors: string[];
}

export interface PaginationResponse<T> {
  items: T[];
  page: number;
  totalPages: number;
  total: number;
  itemsPerPage: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
