using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Configuration;

namespace NRZMyk.Services.Data
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context,
            ILoggerFactory loggerFactory, DatabaseSeed databaseSeed, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                context.Database.Migrate();

                if (!await context.ClinicalBreakpoints.AnyAsync()
                        && databaseSeed?.ClinicalBreakpoints?.Any() == true)
                {
                    await context.ClinicalBreakpoints.AddRangeAsync(databaseSeed.ClinicalBreakpoints);

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<ApplicationDbContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(context, loggerFactory, databaseSeed, retryForAvailability);
                }
                throw;
            }
        }
    }
}