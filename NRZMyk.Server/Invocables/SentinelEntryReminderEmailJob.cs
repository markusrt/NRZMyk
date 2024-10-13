using Coravel.Invocable;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NRZMyk.Services.Interfaces;
using NRZMyk.Server.Utils;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;

namespace NRZMyk.Server.Invocables;

public class SentinelEntryReminderEmailJob : IInvocable
{
    private readonly ILogger<SentinelEntryReminderEmailJob> _logger;
    private readonly IAsyncRepository<Organization> _organizationRepository;
    private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly JobSetting _jobSetting;

    public SentinelEntryReminderEmailJob(
        ILogger<SentinelEntryReminderEmailJob> logger, IOptions<ApplicationSettings> config, IAsyncRepository<Organization> organizationRepository, IAsyncRepository<SentinelEntry> sentinelEntryRepository,
        IEmailNotificationService emailNotificationService)
    {
        _organizationRepository = organizationRepository;
        _sentinelEntryRepository = sentinelEntryRepository;
        _emailNotificationService = emailNotificationService;
        _logger = logger;
        _jobSetting = config?.Value.Application?.SentinelReminderJob ?? JobSetting.Disabled;
    }
 
    public async Task Invoke()
    {
        if (_jobSetting == JobSetting.Disabled)
        {
            _logger.LogInformation($"Sentinel reminder job was skipped at {DateTime.UtcNow} (disabled)");
            return;
        }
        
        var numberOfRemindedOrganizations = 0;
        foreach (var organization in await _organizationRepository.ListAllWithDatesAsync(_sentinelEntryRepository))
        {
            var today = DateTime.Today;
            var isDueThisMonth = organization.DispatchMonth == (MonthToDispatch)today.Month;
            var sentThisMonth = organization.LastReminderSent?.Year == today.Year &&
                                organization.LastReminderSent?.Month == today.Month; 
            if (isDueThisMonth && !sentThisMonth)
            {
                await _emailNotificationService.RemindOrganizationOnDispatchMonth(organization);
                organization.LastReminderSent = today;
                await _organizationRepository.UpdateAsync(organization);
                numberOfRemindedOrganizations++;
            }
        }
        _logger.LogInformation($"Sentinel reminder job invoked at {DateTime.UtcNow} for {numberOfRemindedOrganizations} organizations");
    }
}