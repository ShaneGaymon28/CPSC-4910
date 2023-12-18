using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;

namespace Team22.Web.Services;

public class UserService
{
    private readonly Team22Context _context;

    public UserService(Team22Context context)
    {
        _context = context;
    }

    #region Add User Service

    public class AddUserQuery
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
        public UserRole UserRole { get; init; }
    }
    
    public async Task<QueryResult<int>> Add(AddUserQuery request) {
        // check if email exists
        if (_context.Users.Any(u => u.Email == request.Email.ToLower()))
        {
            return QueryResult<int>.Conflict();
        }
        // make sure request.UserRole is valid
        if (!Enum.IsDefined(typeof(UserRole), request.UserRole))
        {
            return QueryResult<int>.Invalid();
        }

        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            Password = request.Password,
            UserRole = request.UserRole
        };

        // create audit
        var audit = new Audit
        {
            AuditType = AuditType.UserCreate,
            AuditExtra = $"Created new user {newUser.FirstName} {newUser.LastName} with email {newUser.Email} and role {newUser.UserRole}"
        };

        // add to user's audit subject log
        newUser.SubjectEntries.Add(audit);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return QueryResult<int>.Success(newUser.Id);
    }

    #endregion

    #region Get User Service

    public class GetUserQuery
    {
        public string UserName { get; set; }
        public string? Email { get; set; }
    }

    public async Task<QueryResult<AppUser>> GetUser(GetUserQuery request)
    {
        var user = await _context.AppUser.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user is null)
        {
            return QueryResult<AppUser>.NotFound();
        }

        return QueryResult<AppUser>.Success(user);
    }
    
    public async Task<QueryResult<AppUser>> GetUserByEmail(GetUserQuery request)
    {
        var user = await _context.AppUser.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            return QueryResult<AppUser>.NotFound();
        }

        return QueryResult<AppUser>.Success(user);
    }

    public async Task<UserRole> GetUserRole(GetUserQuery request)
    {
        // leaving this relatively unchanged for now
        // having type conversion issues in AccountController, Login
        var user = await _context.AppUser.FirstOrDefaultAsync(u => u.UserName == request.UserName);

        if (user is null) return 0;
        return user.UserRole;
    }

    public async Task<QueryResult<List<AppUser>>> GetAllUsers()
    {
        return QueryResult<List<AppUser>>.Success(await _context.AppUser.ToListAsync());
    }

    #endregion
    
    #region Get Sponsor-User Bridge
    
    public async Task<QueryResult<SponsorUserBridge>> GetSponsorUserBridge(string userId)
    {
        var bridge = await _context.Bridges
            .Include(b => b.AppUser)
            .Include(b => b.Sponsor)
            .FirstOrDefaultAsync(b => b.AppUserId == userId);
        
        if (bridge is null)
        {
            return QueryResult<SponsorUserBridge>.NotFound();
        }
        
        return QueryResult<SponsorUserBridge>.Success(bridge);
    }

    public async Task<QueryResult<List<SponsorUserBridge>>> GetSponsorUserBridgeBySponsorId(int sponsorId)
    {
        var bridge = await _context.Bridges
            .Include(b => b.AppUser)
            .Include(b => b.Sponsor)
            .Where(b => b.SponsorId == sponsorId && b.AppUser.UserRole == UserRole.Driver)
            .ToListAsync();

        // no bridges found
        if (bridge.Count < 1)
        {
            return QueryResult<List<SponsorUserBridge>>.NotFound();
        }
        
        return QueryResult<List<SponsorUserBridge>>.Success(bridge);
    }
    
    #endregion

    #region Update User Service

    public class UpdateUserQuery
    {
        public string Id { get; init; }

        public string UserName { get; init; } = null!;
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public UserRole UserRole { get; init; }
    }

    public async Task<QueryStatus> Update(UpdateUserQuery request)
    {
        // get user
        var user = await _context.AppUser.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user is null)
        {
            return QueryStatus.NotFound;
        }

        // update user
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        // user.UserRole --> read-only
        // user.UserName --> read-only

        // create audit
        var audit = new Audit
        {
            AuditType = AuditType.UserUpdated,
            AuditExtra = $"Updated user {user.FirstName} {user.LastName} with email {user.Email} and role {user.UserRole}"
        };

        // add to user's audit subject log
        // user.SubjectEntries.Add(audit);

        // save changes
        await _context.SaveChangesAsync();
        return QueryStatus.NoContent;
    }
    #endregion

    #region Get drivers associated with a sponsor

    public async Task<QueryResult<List<AppUser>>> GetDriversBySponsorId(int sponsorId)
    {
        var sponsor = await _context.Sponsors.FirstOrDefaultAsync(s => s.Id == sponsorId);
        if (sponsor is null)
        {
            return QueryResult<List<AppUser>>.NotFound();
        }

        var drivers = await _context.Bridges
            .Where(s => s.SponsorId == sponsorId)
            .Select(s => s.AppUser)
            .ToListAsync();

        return QueryResult<List<AppUser>>.Success(drivers);
    }

    #endregion

    #region Driver Drop Sponsor

    public async Task<QueryStatus> DriverDropSponsor(string driverId, int sponsorId)
    {
        var bridge = await _context.Bridges
            .FirstOrDefaultAsync(b => b.AppUserId == driverId && b.SponsorId == sponsorId);
        if (bridge is null)
        {
            return QueryStatus.NotFound;
        }

        _context.Bridges.Remove(bridge);
        await _context.SaveChangesAsync();

        return QueryStatus.Success;
    }

    #endregion
}