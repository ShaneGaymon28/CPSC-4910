using Microsoft.EntityFrameworkCore;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;

namespace Team22.Web.Services;

public class PointService
{
    private readonly Team22Context _context;

    public PointService(Team22Context context)
    {
        _context = context;
    }
    
    #region GET POINT HISTORY

    public async Task<QueryResult<List<Point>>> GetDriverPointHistory(string driverId)
    {
        var points = await _context.Points
            .Where(p => p.DriverId == driverId)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
        
        // no point history found
        if (points.Count < 1)
        {
            return QueryResult<List<Point>>.NotFound();
        }
        
        return QueryResult<List<Point>>.Success(points);
    }


    #endregion
    
    #region ADD NEW POINT CHANGE
    
    public async Task<QueryResult<QueryStatus>> AddNewPointChange(int pointChange, string driverId, string sponsorUserId, int currentPoints, string reason)
    {
        // need to update bridge to current points
        var driver = _context.AppUser.FirstOrDefault(d => d.Id == driverId);
        var sponsorUser = _context.AppUser.FirstOrDefault(d => d.Id == sponsorUserId);

        var pointsAfterChange = currentPoints + pointChange;
        
        var newPointChange = new Point
        {
            CurrentPoints = pointsAfterChange,
            DeltaPoints = pointChange,
            DriverId = driverId,
            SponsorId = sponsorUserId,
            Date = DateTime.Now,
            Reason = reason,
            SponsorOrg = "" // TODO: update 
        };

        var audit = new Audit
        {
            AuditType = pointChange > 0 ? AuditType.PointsAdd : AuditType.PointsRemove,
            AuditExtra =
                $"{sponsorUser.FirstName} {sponsorUser.LastName} updated driver {driver.FirstName} {driver.LastName} points {pointChange} on {DateTime.Now} for {reason}"
        };
        
        // TODO: add audit to db

        // update bridge
        var bridge = _context.Bridges.FirstOrDefault(b => b.AppUserId == driverId);
        bridge.Points = pointsAfterChange;
        
        _context.Points.Add(newPointChange);
        await _context.SaveChangesAsync();
        
        return QueryResult<QueryStatus>.Success();
    }
    
    #endregion

    #region Get total points for user
    // these aren't updated to use AppUser...

    public async Task<QueryResult<int>> GetTotalPoints(int userId)
    {
        var user = await _context.AppUser.FindAsync(userId);

        if (user == null)
        {
            return QueryResult<int>.NotFound();
        }
        
        // Add up all points associated with user
        var totalPoints = _context.Points.Where(p => p.Driver == user).Sum(p => p.DeltaPoints);

        return QueryResult<int>.Success(totalPoints);
    }
    
    #endregion

    #region Add points to user
    // not updated to user app user
    public class AddPointsQuery
    {
        public int DriverId { get; set; }
        public int SponsorId { get; set; }
        public int DeltaPoints { get; set; }
        public string ?Reason { get; set; }
    }

    public async Task<QueryStatus> AddPoints(AddPointsQuery query)
    {
        {
            // Get current points
            var current = await GetTotalPoints(query.DriverId);

            if (current.Status == QueryStatus.NotFound)
            {
                return QueryStatus.NotFound;
            }

            // get user
            AppUser user = await _context.AppUser.FindAsync(query.DriverId);

            // get sponsor
            AppUser sponsor = await _context.AppUser.FindAsync(query.SponsorId);

            // Create new point object
            var point = new Point
            {
                Driver = user,
                CurrentPoints = current.Value,
                DeltaPoints = query.DeltaPoints,
                Date = DateTime.Now,
                Reason = query.Reason,
                Sponsor = sponsor,
                SponsorOrg = "Team22"
            };

            // Add point to database
            await _context.Points.AddAsync(point);
            await _context.SaveChangesAsync();

            return QueryStatus.Success;
        }
    }

    #endregion
}