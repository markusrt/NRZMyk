using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Tests.Utils;
using NSubstitute;
using NUnit.Framework;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NRZMyk.Services.Tests.Services
{
    public class EmailNotificationServiceTests
    {
        [Test]
        public async Task WhenNewUserRegistered_SendsNotificationWithCorrespondingTemplate()
        {
            SendGridMessage messageSent = null;
            
            var sut = CreateSut(out var sendGridClient, out var logger);
            sendGridClient.SendEmailAsync(Arg.Do<SendGridMessage>(s => messageSent = s))
                .Returns(Task.FromResult(new Response(HttpStatusCode.Accepted, null, null)));

            await sut.NotifyNewUserRegistered("Anders Hellman", "anders.hellman@arasaka.nc", "Night City").ConfigureAwait(true);

            messageSent.TemplateId.Should().Be("RegisterTemplate");
            messageSent.From.Email.Should().Be("server@arasaka.nc");
            messageSent.Personalizations.Should().HaveCount(1);
            var personalization = messageSent.Personalizations.First();
            personalization.TemplateData.Should().BeEquivalentTo(new NewUserRegisteredNotification
            {
                UserName = "Anders Hellman",
                UserEmail = "anders.hellman@arasaka.nc",
                UserCity = "Night City",
                Date = DateTime.Now.ToString("dd.MM.yyyy"),
                Time = DateTime.Now.ToString("HH:mm")
            });
            logger.Messages[LogLevel.Information].Should().NotBeEmpty();
        }

        [Test]
        public async Task WhenSendGridNotificationFails_ErrorIsLogged()
        {
            SendGridMessage messageSent = null;

            var sut = CreateSut(out var sendGridClient, out var logger);
            var badResponse = new Response(HttpStatusCode.BadRequest, new ByteArrayContent(Encoding.ASCII.GetBytes("Error details")), null);
            sendGridClient.SendEmailAsync(Arg.Do<SendGridMessage>(s => messageSent = s))
                .Returns(Task.FromResult(badResponse));

            await sut.NotifyNewUserRegistered("荒坂 三郎", "saburo.arasaka@arasaka.nc", "Night City").ConfigureAwait(true);

            logger.Messages[LogLevel.Error].Should().Contain("Error details");
        }

        private EmailNotificationService CreateSut(out ISendGridClient sendGridClient, out TestLogger<EmailNotificationService> logger)
        {
            sendGridClient = Substitute.For<ISendGridClient>();
            logger = new TestLogger<EmailNotificationService>();
            return new EmailNotificationService(sendGridClient, logger, Options.Create(new ApplicationSettings {Application = new Application
            {
                SendGridSenderEmail = "server@arasaka.nc",
                AdministratorEmail = "admin@arasaka.nc",
                SendGridDynamicTemplateId = "RegisterTemplate"
            }}));
        }
    }
}