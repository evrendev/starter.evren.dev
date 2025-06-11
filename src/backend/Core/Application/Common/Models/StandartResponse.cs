namespace EvrenDev.Application.Common.Models;

public class StandartResponse<T>
{
    public bool Succeeded { get; private set; }
    public T Data { get; private set; }
    public string[] Errors { get; private set; }

    private StandartResponse(bool succeeded, T data = default!, string[] errors = null!)
    {
        Succeeded = succeeded;
        Data = data;
        Errors = errors ?? Array.Empty<string>();
    }

    public static StandartResponse<T> Success(T data)
    {
        return new StandartResponse<T>(true, data);
    }

    public static StandartResponse<T> Failure(params string[] errors)
    {
        return new StandartResponse<T>(false, errors: errors);
    }
}
