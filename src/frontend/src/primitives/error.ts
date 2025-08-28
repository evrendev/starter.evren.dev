import type { AxiosError } from "axios";

export type ResponseError = AxiosError;

export const ErrorType = {
  Failure: 0,
  Validation: 1,
  NotFound: 2,
  Conflict: 3,
} as const;

export type ErrorTypeT = (typeof ErrorType)[keyof typeof ErrorType];

export class AppError {
  public readonly message: string;
  public readonly errorType: ErrorTypeT;

  private constructor(message: string, errorType: ErrorTypeT) {
    this.message = message;
    this.errorType = errorType;
  }

  public static notFound(message: string): AppError {
    return new AppError(message, ErrorType.NotFound);
  }

  public static conflict(message: string): AppError {
    return new AppError(message, ErrorType.Conflict);
  }

  public static validation(message: string): AppError {
    return new AppError(message, ErrorType.Validation);
  }

  public static failure(message: string): AppError {
    return new AppError(message, ErrorType.Failure);
  }
}
