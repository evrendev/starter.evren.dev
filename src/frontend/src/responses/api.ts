export interface DefaultApiResponse<T> {
  succeeded: boolean;
  data?: T;
  errors?: string[];
}
