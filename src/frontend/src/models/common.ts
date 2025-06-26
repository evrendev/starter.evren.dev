export interface ApiResponse<T> {
  succeeded: boolean;
  data: T;
  errors: string[];
}

export interface ProblemDetails {
  type: string | null;
  title: string | null;
  status: number | null;
  detail: string | null;
  instance: string | null;
}

export interface HttpValidationProblemDetails extends ProblemDetails {
  errors: { [key: string]: string[] };
}

export interface ErrorResult {
  messages: string[] | null;
  source: string | null;
  exception: string | null;
  errorId: string | null;
  supportMessage: string | null;
  statusCode: number;
}
