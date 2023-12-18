using Microsoft.EntityFrameworkCore;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;
namespace Team22.Web.Services;

public class SponsorService
{
    private readonly Team22Context _context;

    public SponsorService(Team22Context context)
    {
        _context = context;
    }

    #region Add Sponsor Service

    public class AddSponsorQuery
    {
        public int UserId { get; set; } // the user adding the sponsor
        public string Name { get; set; } = null!;
    }

    public async Task<QueryStatus> AddSponsor(AddSponsorQuery request)
    {
        if (_context.Sponsors.Any(s => s.Name == request.Name)) // a sponsor w/ this name already exists
        {
            return QueryStatus.Conflict;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user is null) // invalid userid
        {
            return QueryStatus.NotFound;
        }

        if (user.UserRole != UserRole.Sponsor && user.UserRole != UserRole.Admin) // a user who is not a sponsor or an admin should not be able to make a sponsor
        {
            return QueryStatus.Forbidden;
        }

        var sponsor = new Sponsor
        {
            Name = request.Name
            // TODO: include userId when that gets updated
        };
        
        sponsor.Users.Add(new SponsorUserBridge
        {
            AppUserId = user.Id.ToString(), // TODO: rework once migration 
            SponsorId = sponsor.Id
        });
        user.SubjectEntries.Add(new Audit
        {
            AuditType = AuditType.SponsorCreated,
            AuditExtra = $"Sponsor {sponsor.Name} created."
        });
        
        _context.Sponsors.Add(sponsor);
        await _context.SaveChangesAsync();
        return QueryStatus.NoContent;
    }
    
    #endregion

    #region Get Sponsor Service

    public class GetSponsorQuery
    {
        // only choose one of these to fill.
        public int? SponsorId { get; set; }
        public string? SponsorName { get; set; }
    }

    public async Task<QueryResult<Sponsor>> GetSponsor(GetSponsorQuery request)
    {
        if (request.SponsorId is not null)
        {
            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.Id == request.SponsorId);
            if (sponsor is null)
            {
                return QueryResult<Sponsor>.NotFound();
            }

            return QueryResult<Sponsor>.Success(sponsor);
        }

        if (request.SponsorName is not null)
        {
            var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.Name == request.SponsorName);
            if (sponsor is null)
            {
                return QueryResult<Sponsor>.NotFound();
            }

            return QueryResult<Sponsor>.Success(sponsor);
        }

        return QueryResult<Sponsor>.Invalid();
    }

    #endregion

    #region Get All Sponsors Service

    public async Task<QueryResult<List<Sponsor>>> GetAllSponsors()
    {
        return QueryResult<List<Sponsor>>.Success(await _context.Sponsors.ToListAsync());
    }

    #endregion

    #region Update Sponsor Service

    public class UpdateSponsorQuery
    {
        public int UserId { get; set; } // the user updating the sponsor
        public int SponsorId { get; set; } // the sponsor being updated
        public bool AcceptingApps { get; set; }
        public string? Name { get; set; }
        public double PointDollarRatio { get; set; }
    }

    public async Task<QueryStatus> UpdateSponsor(UpdateSponsorQuery request)
    {
        var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.Id == request.SponsorId);
        if (sponsor is null)
        {
            return QueryStatus.NotFound;
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user is null)
        {
            return QueryStatus.NotFound;
        }

        if (user.UserRole != UserRole.Admin) // only admins can update sponsors
        {
            return QueryStatus.Forbidden;
        }

        if (request.Name is not null)
        {
            sponsor.Name = request.Name;
        }

        sponsor.AcceptingApps = request.AcceptingApps;

        if (request.PointDollarRatio != 0)
        {
            sponsor.PointDollarRatio = request.PointDollarRatio;
        }

        await _context.SaveChangesAsync();
        return QueryStatus.NoContent;
    }

    #endregion

    #region Get Available Sponsors

    public async Task<List<Sponsor>> GetAvailableSponsors()
    {
        return await _context.Sponsors.Where(s => s.AcceptingApps).ToListAsync();
    }

    #endregion
}