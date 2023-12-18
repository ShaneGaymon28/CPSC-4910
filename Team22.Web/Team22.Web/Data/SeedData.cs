using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;

/*
 * This class is to seed the DB with information
 *
 * NOTE: the DB schema will change, DON'T DELETE THIS CLASS, just comment out the section of code in
 *      Program.cs and I will update it
 *
 * If you're getting errors when running, DROP/DELETE the DB and re-run it 
 */

namespace Team22.Web.Data;

public class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new Team22Context(serviceProvider.GetRequiredService<DbContextOptions<Team22Context>>()))
        {
            #region Sponsors
            
            if (!context.Sponsors.Any())
            {
                context.Sponsors.AddRange(
                    new Sponsor
                    {
                        Id = 1,
                        Name = "Amazon Prime",
                        PointDollarRatio = 0.01,
                        AcceptingApps = true
                    },
                    new Sponsor
                    {
                        Id = 2,
                        Name = "Walmart",
                        PointDollarRatio = 0.05,
                        AcceptingApps = true
                    });

                context.SaveChanges();
            }

            #endregion
            
            
            #region Catalogs
            if (!context.Catalog.Any())
            {
                context.Catalog.AddRange(
                    new Catalog
                    {
                        Id = 1,
                        SponsorId = 1,
                    },
                    new Catalog
                    {
                        Id = 2,
                        SponsorId = 2,
                    });

                context.SaveChanges();
            }
            
            #endregion
        }
    }
}