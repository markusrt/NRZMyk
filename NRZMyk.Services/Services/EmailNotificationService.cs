using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Models;
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
            var sendGridMessage = new SendGridMessage();
            sendGridMessage.SetFrom(_appSettings.SendGridSenderEmail);
            sendGridMessage.AddTo(_appSettings.AdministratorEmail);
            sendGridMessage.SetTemplateId(_appSettings.SendGridDynamicTemplateId);
            sendGridMessage.SetTemplateData(new NewUserRegisteredNotification
            {
                UserName = userName,
                UserEmail = userEmail,
                UserCity = userCity,
                Date = now.ToString("dd.MM.yyyy"),
                Time = now.ToString("HH:mm")
            });

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogInformation($"Email notification on new user registration was sent via SendGrid to {_appSettings.AdministratorEmail}");
            }
            else
            {
                var errorDetails = await response.Body.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError($"Email notification via SendGrid failed with status {response.StatusCode}, error details: '{errorDetails}'");

            }
        }

        public void SendEmail(string email, string message)
        {
        }
    }
}