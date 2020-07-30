using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NRZMyk.Services.Data
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext catalogContext,
            ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            try
            {
                // TODO: Only run this if using a real database
                catalogContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<ApplicationDbContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(catalogContext, loggerFactory, retryForAvailability);
                }
                throw;
            }
        }
    }
}