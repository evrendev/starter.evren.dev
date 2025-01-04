namespace EvrenDev.Application.Common.Models;
public class Result<T>
{
    public bool Succeeded { get; private set; }
    public T Data { get; private set; }
    public string[] Errors { get; private set; }

    private Result(bool succeeded, T data = default!, string[] errors = null!)
    {
        Succeeded = succeeded;
        Data = data;
        Errors = errors ?? Array.Empty<string>();
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data);
    }

    public static Result<T> Failure(params string[] errors)
    {
        return new Result<T>(false, errors: errors);
    }
}