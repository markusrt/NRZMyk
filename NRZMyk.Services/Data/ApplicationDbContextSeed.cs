using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Data
{
    [ExcludeFromCodeCoverage(Justification = "Not testable with in memory DB")]
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context,
            ILoggerFactory loggerFactory, DatabaseSeed databaseSeed, IClinicalBreakpointProvider breakpointProvider, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                if (context.Database.IsRelational())
                {
                    await context.Database.MigrateAsync();
                }

                var configuredBreakpoints = breakpointProvider.GetBreakpoints();
                if (configuredBreakpoints.Any())
                {
                    var databaseCount = await context.ClinicalBreakpoints.CountAsync();
                    if (databaseCount < configuredBreakpoints.Count)
                    {
                        var databaseBreakpoints = await context.ClinicalBreakpoints.ToListAsync();
                        var newBreakpoints = configuredBreakpoints.Where(
                            breakpoint => !databaseBreakpoints.Any(
                                b => b.AntifungalAgent == breakpoint.AntifungalAgent
                                     && b.Standard == breakpoint.Standard
                                     && b.Version == breakpoint.Version
                                     && b.AntifungalAgentDetails == breakpoint.AntifungalAgentDetails)
                            ).ToList();
                        await context.ClinicalBreakpoints.AddRangeAsync(newBreakpoints).ConfigureAwait(false);
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }
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
                    await SeedAsync(context, loggerFactory, databaseSeed, breakpointProvider, retryForAvailability).ConfigureAwait(false);
                }
                throw;
            }
        }
    }
}