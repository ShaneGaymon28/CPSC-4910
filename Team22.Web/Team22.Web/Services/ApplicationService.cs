using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;

namespace Team22.Web.Services;

public class ApplicationService
{
    private readonly Team22Context _context;
    private readonly UserService _userService;
    private readonly SponsorService _sponsorService;

    public ApplicationService(Team22Context context, UserService userService, SponsorService sponsorService)
    {
        _context = context;
        _userService = userService;
        _sponsorService = sponsorService;
    }

    #region Add Application Service

    public class AddApplicationQuery
    {
        public string UserName { get; set; }
        public string SponsorName { get; set; }
        public string Reason { get; set; }
    }
    
    public async Task<QueryStatus> AddApplication(AddApplicationQuery request)
    {
        var user = _context.AppUser.FirstOrDefault(u => u.UserName == request.UserName);
        if (user is null) { return QueryStatus.NotFound; }
        
        var sponsor = _context.Sponsors.FirstOrDefault(s => s.Name == request.SponsorName);
        if (sponsor is null) { return QueryStatus.NotFound; }

        var newApp = new Application
        {
            UserId = user.Id,
            User = user,
            SponsorId = sponsor.Id,
            Sponsor = sponsor,
            Reason = request.Reason,
            DriverId = user.Id
        };
        
        // ADD CHECK FOR EXISTING APPLICATION

        // add to user's audit list
        /*user.SubjectEntries.Add(new Audit
        {
            AuditType = AuditType.DriverApplicationSubmitted,
            AuditExtra = $"Application submitted to sponsor {sponsor.Name}"
        });*/

        _context.Application.Add(newApp);
        await _context.SaveChangesAsync();
        return QueryStatus.Success;
    }

    #endregion

    #region Get Application Service

    public async Task<QueryResult<Application>> GetApplication(int appId)
    {
        var app = await _context.Application
            .Include(a => a.User)
            .Include(a => a.Sponsor)
            .FirstOrDefaultAsync(a => a.Id == appId);

        return QueryResult<Application>.Success(app);
    }
    
    public async Task<QueryResult<List<Application>>> GetAllApplications()
    {
        var apps = await _context.Application
            .Include(a => a.User)
            .Include(a => a.Sponsor)
            .ToListAsync();

        return QueryResult<List<Application>>.Success(apps);
    }

    public async Task<QueryResult<List<Application>>> GetApplicationsBySponsorId(int sponsorId)
    {
        var apps = await _context.Application
            .Include(a => a.User)
            .Include(a => a.Sponsor)
            .Where(a=>a.SponsorId == sponsorId)
            .ToListAsync();

        return QueryResult<List<Application>>.Success(apps);
    }

    public async Task<QueryResult<List<Application>>> GetApplicationsByDriverId(string driverId)
    {
        var apps = await _context.Application
            .Include(a => a.User)
            .Include(a => a.Sponsor)
            .Where(a => a.UserId == driverId)
            .ToListAsync();

        return QueryResult<List<Application>>.Success(apps);
    }
    

    #endregion

    #region Update Application Service

    public class UpdateApplicationQuery
    {
        public int AppId { get; set; }
        public string DeciderId { get; set; } // the user that accepts/rejects
        public bool Accepted { get; set; }
        public string? Reason { get; set; } = null!;
    }

    
    public async Task<QueryStatus> UpdateApplication(UpdateApplicationQuery request)
    {
        // get application associated with appid
        var app = _context.Application
            .Include(a => a.Sponsor)
            .Include(a => a.User)
            .FirstOrDefault(a => a.Id == request.AppId);

        // check if application exists
        if (app is null) { return QueryStatus.NotFound; }
        
        var decider = _context.AppUser.FirstOrDefault(u => u.Id == request.DeciderId);
        
        // check that the decider exists
        if (decider is null) { return QueryStatus.NotFound; }

        // make sure they are in a position to accept/deny
        if (decider.UserRole == UserRole.Driver) { return QueryStatus.Forbidden; }

        // get sponsor and user
        // need to update to use bridge
        var sponsor = app.Sponsor;
        var user = app.User;

        // update application and user
        app.Status = request.Accepted ? ApplicationStatus.Accepted : ApplicationStatus.Rejected;
        app.Reason = request.Reason;
        var audit = new Audit
        {
            AuditType = request.Accepted ? AuditType.DriverApplicationAccepted : AuditType.DriverApplicationRejected,
            AuditExtra = $"Application {(request.Accepted ? "accepted" : "rejected")} for sponsor {sponsor.Name}"
        };
        //user.SubjectEntries.Add(audit);
        // need to either add AuthorEntries to AppUser or figure out some other way for this to work
        //decider.AuthorEntries.Add(audit);

        // I'm not sure this is needed
        
        /*if (request.Accepted)
        {
            var bridge = new SponsorUserBridge
            {
                SponsorId = sponsor.Id,
                AppUserId = user.Id.ToString(), // TODO: rework this once I can confirm migration is good
                Points = 0
            };
            
            user.Sponsors.Add(bridge);
            sponsor.Users.Add(bridge);
        }*/

        // save changes and finish
        await _context.SaveChangesAsync();
        return QueryStatus.NoContent;
    }

    public async Task<QueryStatus> DropApplication(UpdateApplicationQuery request)
    {
        var application = GetApplication(request.AppId);
        
        var app = _context.Application.Remove(application.Result.Value);

        await _context.SaveChangesAsync();
        return QueryStatus.NoContent;
    }
    
    

    #endregion
}