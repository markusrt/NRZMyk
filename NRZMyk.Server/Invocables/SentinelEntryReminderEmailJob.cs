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
    private IReminderService _reminderService;
    private readonly ILogger<SentinelEntryReminderEmailJob> _logger;
    private readonly IAsyncRepository<Organization> _organizationRepository;
    private readonly IAsyncRepository<SentinelEntry> _sentinelEntryRepository;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly JobSetting _jobSetting;

    public SentinelEntryReminderEmailJob(
        ILogger<SentinelEntryReminderEmailJob> logger, IOptions<ApplicationSettings> config, IReminderService reminderService, 
        IAsyncRepository<Organization> organizationRepository, IAsyncRepository<SentinelEntry> sentinelEntryRepository,
        IEmailNotificationService emailNotificationService)
    {
        _reminderService = reminderService;
        _organizationRepository = organizationRepository;
        _sentinelEntryRepository = sentinelEntryRepository;
        _emailNotificationService = emailNotificationService;
        _logger = logger;
        _jobSetting = config?.Value?.Application?.SentinelReminderJob ?? JobSetting.Disabled;
    }
 
    public async Task Invoke()
    {
        if (_jobSetting == JobSetting.Disabled)
        {
            _logger.LogInformation($"Sentinel reminder job was skipped invoked at {DateTime.UtcNow} (disabled)");
            return;
        }
        
        var numberOfRemindedOrganizations = 0;
        foreach (var organization in await _organizationRepository.ListAllWithDatesAsync(_sentinelEntryRepository))
        {
            var expectedNextSending = _reminderService.CalculateExpectedNextSending(organization);

            if (DateTime.Today > expectedNextSending)
            {
                var message = $"Data was entered, but no strain has arrived yet from {organization.Name}";
                await _emailNotificationService.SendEmail(organization.Email, message);
                numberOfRemindedOrganizations++;
            }
        }
        _logger.LogInformation($"Sentinel reminder job invoked at {DateTime.UtcNow} for {numberOfRemindedOrganizations} organizations");
    }

    /*
       [Test]
       public void CheckDataAndSendReminders_DataEntered_NoStrainArrived_SendEmail()
       {
           var emailNotificationServiceMock = Substitute.For<IEmailNotificationService>();
           var sut = CreateSut(emailNotificationServiceMock);
           var org = CreateOrganization();
           org.DispatchMonth = MonthToDispatch.January;
           org.LatestSamplingDate = new DateTime(2023, 1, 5);
           org.LatestCryoDate = new DateTime(2022, 12, 20);

           sut.CheckDataAndSendReminders(org);

           emailNotificationServiceMock.Received(1).SendEmail("example@example.com", "Data was entered, but no strain has arrived yet.");
       }
     */
}