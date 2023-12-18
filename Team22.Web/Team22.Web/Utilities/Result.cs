namespace Team22.Web.Utilities;

public class Result<T, U> : IResult<T, U>
{
    public T? Value { get; }

    public U Status { get; }

    public Result(T? value, U status)
    {
        Value = value;
        Status = status;
    }
}