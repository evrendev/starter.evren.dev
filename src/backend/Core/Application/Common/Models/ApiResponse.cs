namespace EvrenDev.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Succeeded { get; private set; }
    public T Data { get; private set; }
    public string[] Errors { get; private set; }

    private ApiResponse(bool succeeded, T data = default!, string[] errors = null!)
    {
        Succeeded = succeeded;
        Data = data;
        Errors = errors ?? Array.Empty<string>();
    }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T>(true, data);
    }

    public static ApiResponse<T> Failure(params string[] errors)
    {
        return new ApiResponse<T>(false, errors: errors);
    }
}
