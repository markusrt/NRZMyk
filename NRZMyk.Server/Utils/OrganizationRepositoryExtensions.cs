using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Specifications;

namespace NRZMyk.Server.Utils;

public static class OrganizationRepositoryExtensions
{
    public static async Task<IReadOnlyList<Organization>> ListAllWithDatesAsync(
        this IAsyncRepository<Organization> organizationRepository,
        IAsyncRepository<SentinelEntry> sentinelEntryRepository)
    {
        var organizations = await organizationRepository.ListAllAsync().ConfigureAwait(false);
        foreach (var organization in organizations)
        {
            var latestEntryBySamplingDate =
                await sentinelEntryRepository.FirstOrDefaultAsync(new SentinelEntryBySamplingDateSpecification(1, $"{organization.Id}"));
            organization.LatestSamplingDate = latestEntryBySamplingDate?.SamplingDate;

            var latestEntryByCryoDate =
                await sentinelEntryRepository.FirstOrDefaultAsync(new SentinelEntryByCryoDateSpecification(1, $"{organization.Id}"));
            organization.LatestCryoDate = latestEntryByCryoDate?.CryoDate;
        }

        return organizations;
    }
}