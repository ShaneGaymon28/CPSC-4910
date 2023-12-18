namespace Team22.Web.Enums;

public enum QueryStatus
{
    Success, // 200 Ok()
    Invalid, // 400 BadRequest()
    NotFound, // 404 NotFound()
    Conflict, // 409 Conflict()
    Forbidden, // 403 Forbid()
    NoContent // 204 NoContent()
    ,
    Succeeded
}