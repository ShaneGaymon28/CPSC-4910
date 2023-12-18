using Team22.Web.Enums;

namespace Team22.Web.Utilities;

public class QueryResult<T> : IResult<T, QueryStatus>
{
    public T? Value { get; }
    public QueryStatus Status { get; }

    public QueryResult(QueryStatus status)
    {
        Status = status;
    }

    public QueryResult(T? value, QueryStatus status)
    {
        Value = value;
        Status = status;
    }

    public static QueryResult<T> Success(T? value = default) => new(value, QueryStatus.Success);

    public static QueryResult<T> Invalid(T? value = default) => new(value, QueryStatus.Invalid);

    public static QueryResult<T> NotFound() => new(default, QueryStatus.NotFound);

    public static QueryResult<T> Conflict() => new(default, QueryStatus.Conflict);

    public static QueryResult<T> Forbidden() => new(default, QueryStatus.Forbidden);

    public static QueryResult<T> NoContent() => new(default, QueryStatus.NoContent);
}