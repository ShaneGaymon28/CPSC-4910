using Team22.Web.Enums;
namespace Team22.Web.Models;

public class Audit
{
    public int Id { get; set; }
    
    // need to get from Team22.Api proj
    public AuditType AuditType { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public int? AuthorId { get; set; }
    public User? Author { get; set; }
    
    public int SubjectId { get; set; }
    public User Subject { get; set; } = null!;
    
    // extra info/comments will go here
    public string? AuditExtra { get; set; }
}