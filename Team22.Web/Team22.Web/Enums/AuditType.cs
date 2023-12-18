namespace Team22.Web.Enums;

public enum AuditType
{
    UserCreate,
    UserUpdated,
    UserDelete,
    UserRoleChange,
    UserSponsorChange,
    UserPasswordChange,
    UserVerified,
    PointsAdd,
    PointsRemove,
    LoginSuccess,
    LoginFailure,
    DriverApplicationSubmitted,
    DriverApplicationAccepted,
    DriverApplicationRejected,
    SponsorCreated,
}