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
        IAsyncRepository<SentinelEntry> sentinelEntryRepository,
        bool includeStatistics = false)
    {
        var organizations = await organizationRepository.ListAsync(
            new OrganizationsIncludingRemoteAccountSpecification()).ConfigureAwait(false);
        var currentYear = DateTime.Now.Year;
        
        foreach (var organization in organizations)
        {
            var protectKey = $"{organization.Id}";
            
            var latestEntryByReceivingDate =
                await sentinelEntryRepository.FirstOrDefaultAsync(new SentinelEntryByReceivingDateSpecification(1, protectKey)).ConfigureAwait(false);
            organization.LatestReceivingDate = latestEntryByReceivingDate?.ReceivingDate;

            var latestEntryByCryoDate =
                await sentinelEntryRepository.FirstOrDefaultAsync(new SentinelEntryByCryoDateSpecification(1, protectKey)).ConfigureAwait(false);
            organization.LatestCryoDate = latestEntryByCryoDate?.CryoDate;

            if (includeStatistics)
            {
                // Count total entries created but not stored (no cryo date)
                organization.TotalCreatedNotStoredCount = 
                    await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: false)).ConfigureAwait(false);

                // Count total cryo archived entries (has cryo date)
                organization.TotalCryoArchivedCount = 
                    await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: true)).ConfigureAwait(false);

                // Count current year entries created but not stored
                organization.CurrentYearCreatedNotStoredCount = 
                    await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: false, year: currentYear)).ConfigureAwait(false);

                // Count current year cryo archived entries
                organization.CurrentYearCryoArchivedCount = 
                    await sentinelEntryRepository.CountAsync(new SentinelEntryCountSpecification(protectKey, hasCryoDate: true, year: currentYear)).ConfigureAwait(false);
            }
        }

        return organizations;
    }
}