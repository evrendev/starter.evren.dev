import type { AppError } from './Error'

export class Result<T> {
  public readonly value?: T
  public readonly succeeded: boolean
  public readonly errors?: AppError

  private constructor(value?: T, errors?: AppError) {
    if (errors) {
      this.errors = errors
      this.succeeded = false
    } else {
      this.value = value
      this.succeeded = true
    }
  }

  public static success<T>(value: T): Result<T> {
    return new Result<T>(value)
  }

  public static failure<T>(errors: AppError): Result<T> {
    return new Result<T>(undefined, errors)
  }

  public static create<T>(value: T): Result<T> {
    return new Result<T>(value)
  }

  public get isFailure(): boolean {
    return !this.succeeded
  }
}
