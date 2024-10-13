using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Models.EmailTemplates;
using NRZMyk.Services.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NRZMyk.Services.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly Application _appSettings;

        public EmailNotificationService(ISendGridClient sendGridClient, ILogger<EmailNotificationService> logger, IOptions<ApplicationSettings> config)
        {
            _sendGridClient = sendGridClient;
            _logger = logger;
            _appSettings = config?.Value?.Application ?? new Application();
        }

        public async Task NotifyNewUserRegistered(string userName, string userEmail, string userCity)
        {
            var now = DateTime.Now;
            var newUserRegisteredNotification = new NotifyNewUserRegistered
            {
                UserName = userName,
                UserEmail = userEmail,
                UserCity = userCity,
                Date = now.ToString("dd.MM.yyyy"),
                Time = now.ToString("HH:mm")
            };
            var toAddresses = new List<string> { _appSettings.AdministratorEmail };
            await SendEmail(newUserRegisteredNotification, toAddresses, _appSettings.SendGridSenderEmail, _appSettings.SendGridDynamicTemplateId);
        }

        public async Task RemindOrganizationOnDispatchMonth(Organization organization)
        {
            var remindOrganizationOnDispatchMonth = new RemindOrganizationOnDispatchMonth
            {
                OrganizationName = organization.Name,
                DispatchMonth = EnumUtils.GetEnumDescription(organization.DispatchMonth),
                LatestCryoDate = ReportFormatter.ToReportFormat(organization.LatestCryoDate)
            };
            var toAddresses = new List<string> { _appSettings.AdministratorEmail };
            toAddresses.AddRange(organization.Members.Select(m => m.Email));
            await SendEmail(remindOrganizationOnDispatchMonth, toAddresses, _appSettings.AdministratorEmail, _appSettings.SendGridRemindOrganizationOnDispatchMonthTemplateId);
        }

        private async Task SendEmail(object templateData, List<string> toAddresses, string fromAddress, string templateId)
        {
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom(fromAddress);
            foreach (var address in toAddresses)
            {
                sendGridMessage.AddTo(address);
            }
            sendGridMessage.SetTemplateId(templateId);
            
            sendGridMessage.SetTemplateData(templateData);

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"{templateData.GetType()} email was sent via SendGrid with data {templateData}");
            }
            else
            {
                var errorDetails = await response.Body.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError($"Email notification via SendGrid failed with status {response.StatusCode}, error details: '{errorDetails}'");

            }
        }

        
    }
}