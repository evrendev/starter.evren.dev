import type { AppError } from './Error'

export class Result<T> {
  public readonly data?: T
  public readonly succeeded: boolean
  public readonly errors?: AppError

  private constructor(data?: T, errors?: AppError) {
    if (errors) {
      this.errors = errors
      this.succeeded = false
    } else {
      this.data = data
      this.succeeded = true
    }
  }

  public static success<T>(data: T): Result<T> {
    return new Result<T>(data)
  }

  public static failure<T>(errors: AppError): Result<T> {
    return new Result<T>(undefined, errors)
  }

  public static create<T>(data: T): Result<T> {
    return new Result<T>(data)
  }

  public get isFailure(): boolean {
    return !this.succeeded
  }
}
