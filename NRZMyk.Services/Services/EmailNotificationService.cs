using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;
using brevo_csharp.Api;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Task = System.Threading.Tasks.Task;

namespace NRZMyk.Services.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IBrevoEmailClient _brevoClient;
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly Application _appSettings;

        public EmailNotificationService(IBrevoEmailClient brevoClient, ILogger<EmailNotificationService> logger, IOptions<ApplicationSettings> config)
        {
            _brevoClient = brevoClient;
            _logger = logger;
            _appSettings = config?.Value?.Application ?? new Application();
        }

        public async Task NotifyNewUserRegistered(string userName, string userEmail, string userCity)
        {
            var now = DateTime.Now;
            var templateParams = new Dictionary<string, object>
            {
                { "UserName", userName },
                { "UserEmail", userEmail },
                { "UserCity", userCity },
                { "Date", now.ToString("dd.MM.yyyy") },
                { "Time", now.ToString("HH:mm") }
            };
            var toAddresses = new List<string> { _appSettings.AdministratorEmail };
            await SendEmail(templateParams, toAddresses, _appSettings.NewUserRegisteredTemplateId);
        }

        public async Task RemindOrganizationOnDispatchMonth(Organization organization)
        {
            var templateParams = new Dictionary<string, object>
            {
                { "OrganizationName", organization.Name },
                { "DispatchMonth", EnumUtils.GetEnumDescription(organization.DispatchMonth) },
                { "LatestCryoDate", organization.LatestCryoDate.ToReportFormat() }
            };
            var toAddresses = new List<string> { _appSettings.AdministratorEmail };
            toAddresses.AddRange(organization.Members.Select(m => m.Email));
            await SendEmail(templateParams, toAddresses, _appSettings.RemindOrganizationOnDispatchMonthTemplateId);
        }

        private async Task SendEmail(Dictionary<string, object> templateParams, List<string> toAddresses, long templateId)
        {
            var sendSmtpEmail = new SendSmtpEmail(
                to: toAddresses.Select(email => new SendSmtpEmailTo(email: email)).ToList(),
                sender: new SendSmtpEmailSender(email: _appSettings.SenderEmail, name: _appSettings.SenderName),
                templateId: templateId,
                _params: templateParams
            );

            try
            {
                var result = await _brevoClient.SendTransacEmailAsync(sendSmtpEmail).ConfigureAwait(false);
                _logger.LogInformation("Email was sent via Brevo with template ID {templateId} and message ID {messageId}", templateId, result.MessageId);
            }
            catch (ApiException ex)
            {
                _logger.LogError("Email notification via Brevo failed with status {statusCode}, error details: '{errorDetails}'", ex.ErrorCode, ex.Message);
                throw;
            }
        }

        
    }
}