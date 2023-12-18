using Team22.Web.Contexts;
using Team22.Web.Models;

namespace Team22.Web.Services;

// will be called from DashboardController to get any information from the DB
public class DashboardService
{
    private readonly Team22Context _context;
    

    public DashboardService(Team22Context context)
    {
        _context = context;
        
    }

    
    // test function to make sure front end is connected to back end
    // *** FOR THIS TO WORK: you need to add a user in your local DB ***
    public IEnumerable<User> GetAllUsersAsync()
    {
        var result = _context.Users.ToList();
        return result;
        //return _context.Users.ToList();
    }
    
}