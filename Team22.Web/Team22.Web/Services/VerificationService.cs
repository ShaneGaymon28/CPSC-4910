using Microsoft.EntityFrameworkCore;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;

namespace Team22.Web.Services;

public class VerificationService
{
    private readonly Team22Context _context;

    public VerificationService(Team22Context context)
    {
        _context = context;
    }
    
    #region Verify User Service

    public class VerifyUserQuery
    {
        public int UserId { get; init; }
        public Guid Secret { get; init; }
    }
    
    public async Task<QueryStatus> VerifyUser(VerifyUserQuery request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        
        if (user is null) // invalid user
        {
            return QueryStatus.NotFound;
        }

        var verification = user.Verification;

        if (verification.Status != VerificationStatus.EmailSent) // invalid status
        {
            return QueryStatus.Invalid;
        }

        if (request.Secret != verification.Secret) // invalid guid
        {
            return QueryStatus.Conflict;
        }
        
        if (DateTime.UtcNow > verification.ExpirationDate) // expired - todo: regenerate GUID & resend link?
        {
            return QueryStatus.Conflict;
        }

        verification.Status = VerificationStatus.Verified;

        // create audit
        var audit = new Audit
        {
            AuditType = AuditType.UserVerified
        };

        user.SubjectEntries.Add(audit);

        await _context.SaveChangesAsync();
        return QueryStatus.NoContent;
    }

    #endregion

    #region Get Verification Service

    public class GetVerificationQuery
    {
        public int UserId { get; set; }
    }

    public async Task<QueryResult<Verification>> GetVerification(GetVerificationQuery request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user is null)
        {
            return QueryResult<Verification>.NotFound();
        }

        return QueryResult<Verification>.Success(user.Verification);
    }

    #endregion
}