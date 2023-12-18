namespace Team22.Web.Utilities;

public interface IResult<T, U>
{
    T? Value { get; }

    U Status { get; }
}