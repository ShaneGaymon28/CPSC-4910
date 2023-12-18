using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Team22.Web.Models;

namespace Team22.Web.Contexts;

// " : DbContext" must be changed to " : IdentityDbContext<IdentityUser>" to use Identity
public class Team22Context : IdentityDbContext<IdentityUser>
{
    public Team22Context(DbContextOptions<Team22Context> ctx) : base(ctx) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Sponsor> Sponsors { get; set; }
    public DbSet<Audit> Audits { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Application> Application { get; set; }
    public DbSet<Verification> Verification { get; set; }
    public DbSet<PasswordChange> PasswordChanges { get; set; }
    public DbSet<SponsorUserBridge> Bridges { get; set; }
    public DbSet<Catalog> Catalog { get; set; }
    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<Point> Points { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // todo: use for manual SQL stuff
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<Sponsor>()
            .HasIndex(s => s.Name)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasMany(u => u.AuthorEntries)
            .WithOne(a => a.Author)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<User>()
            .HasMany(u => u.SubjectEntries)
            .WithOne(a => a.Subject)
            .OnDelete(DeleteBehavior.NoAction);
        

            base.OnModelCreating(modelBuilder);
    }
}