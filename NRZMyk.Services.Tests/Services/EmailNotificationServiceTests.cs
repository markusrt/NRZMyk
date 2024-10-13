using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Models.EmailTemplates;
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
        public async Task WhenNewUserRegistered_SendsEmailWithCorrespondingTemplate()
        {
            SendGridMessage messageSent = null;
            
            var sut = CreateSut(out var sendGridClient, out var logger);
            sendGridClient.SendEmailAsync(Arg.Do<SendGridMessage>(s => messageSent = s))
                .Returns(Task.FromResult(new Response(HttpStatusCode.Accepted, null, null)));

            await sut.NotifyNewUserRegistered("Anders Hellman", "anders.hellman@arasaka.nc", "Night City");

            messageSent.TemplateId.Should().Be("RegisterTemplate");
            messageSent.From.Email.Should().Be("server@arasaka.nc");
            messageSent.Personalizations.Should().HaveCount(1);
            var personalization = messageSent.Personalizations.First();
            personalization.TemplateData.Should().BeEquivalentTo(new NotifyNewUserRegistered
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
        public async Task WhenOrganizationRemindedOnDispatchMonth_SendsEmailWithCorrespondingTemplate()
        {
            var organization = new Organization
            {
                Name = "Org 1",
                DispatchMonth = MonthToDispatch.October,
                LatestCryoDate = new DateTime(2024, 10, 13),
                Members = new List<RemoteAccount>{new() {Email = "member1@org.de"}, new() {Email = "member2@org.de"}}
            };
            SendGridMessage messageSent = null;
            
            var sut = CreateSut(out var sendGridClient, out var logger);
            sendGridClient.SendEmailAsync(Arg.Do<SendGridMessage>(s => messageSent = s))
                .Returns(Task.FromResult(new Response(HttpStatusCode.Accepted, null, null)));

            await sut.RemindOrganizationOnDispatchMonth(organization);

            messageSent.TemplateId.Should().Be("ReminderTemplate");
            messageSent.From.Email.Should().Be("admin@arasaka.nc");
            messageSent.Personalizations.Should().HaveCount(1);
            var personalization = messageSent.Personalizations.First();
            personalization.TemplateData.Should().BeEquivalentTo(new RemindOrganizationOnDispatchMonth
            {
                OrganizationName = "Org 1",
                DispatchMonth = "Oktober",
                LatestCryoDate = "13.10.2024"
            });
            personalization.Tos.Should().Contain(new EmailAddress("admin@arasaka.nc"));
            personalization.Tos.Should().Contain(new EmailAddress("member1@org.de"));
            personalization.Tos.Should().Contain(new EmailAddress("member2@org.de"));
            logger.Messages[LogLevel.Information].Should().ContainAll("Org 1", "Oktober", "13.10.2024");
            logger.Messages[LogLevel.Information].Should().NotContainAny("@");
        }

        [Test]
        public async Task WhenSendGridNotificationFails_ErrorIsLogged()
        {
            var sut = CreateSut(out var sendGridClient, out var logger);
            var badResponse = new Response(HttpStatusCode.BadRequest, new ByteArrayContent(Encoding.ASCII.GetBytes("Error details")), null);
            sendGridClient.SendEmailAsync(Arg.Do<SendGridMessage>(s => _ = s))
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
                SendGridDynamicTemplateId = "RegisterTemplate",
                SendGridRemindOrganizationOnDispatchMonthTemplateId = "ReminderTemplate"
            }}));
        }
    }
}