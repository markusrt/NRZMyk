using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Data
{
    [ExcludeFromCodeCoverage(Justification = "Not testable with in memory DB")]
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context,
            ILoggerFactory loggerFactory, DatabaseSeed databaseSeed, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                await context.Database.MigrateAsync();

                if (!await context.ClinicalBreakpoints.AnyAsync()
                        && databaseSeed?.ClinicalBreakpoints?.Any() == true)
                {
                    await context.ClinicalBreakpoints.AddRangeAsync(databaseSeed.ClinicalBreakpoints).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
                if (!await context.Organizations.AnyAsync()
                    && !string.IsNullOrEmpty(databaseSeed?.MainOrganization))
                {
                    await context.Organizations.AddAsync(new Organization {Name = databaseSeed.MainOrganization}).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<ApplicationDbContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(context, loggerFactory, databaseSeed, retryForAvailability).ConfigureAwait(false);
                }
                throw;
            }
        }
    }
}