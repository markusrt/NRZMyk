using System;
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
        var organizations = await organizationRepository.ListAsync(
            new OrganizationsIncludingRemoteAccountSpecification()).ConfigureAwait(false);
        var currentYear = DateTime.Now.Year;
        
        foreach (var organization in organizations)
        {
            var protectKey = $"{organization.Id}";
            
            var latestEntryBySamplingDate =
                await sentinelEntryRepository.FirstOrDefaultAsync(new SentinelEntryBySamplingDateSpecification(1, protectKey));
            organization.LatestSamplingDate = latestEntryBySamplingDate?.SamplingDate;

            var latestEntryByCryoDate =
                await sentinelEntryRepository.FirstOrDefaultAsync(new SentinelEntryByCryoDateSpecification(1, protectKey));
            organization.LatestCryoDate = latestEntryByCryoDate?.CryoDate;

            // Count total entries created but not stored (no cryo date)
            organization.TotalCreatedNotStoredCount = 
                await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: false));

            // Count total cryo archived entries (has cryo date)
            organization.TotalCryoArchivedCount = 
                await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: true));

            // Count current period entries created but not stored
            organization.CurrentPeriodCreatedNotStoredCount = 
                await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: false, year: currentYear));

            // Count current period cryo archived entries
            organization.CurrentPeriodCryoArchivedCount = 
                await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: true, year: currentYear));
        }

        return organizations;
    }
}