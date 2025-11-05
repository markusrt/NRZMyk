using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Tests.Utils;
using NSubstitute;
using NUnit.Framework;
using brevo_csharp.Client;
using brevo_csharp.Model;
using Task = System.Threading.Tasks.Task;

namespace NRZMyk.Services.Tests.Services
{
    public class EmailNotificationServiceTests
    {
        [Test]
        public async Task WhenNewUserRegistered_SendsEmailWithCorrespondingTemplate()
        {
            SendSmtpEmail emailSent = null;
            
            var sut = CreateSut(out var brevoClient, out var logger);
            brevoClient.SendTransacEmailAsync(Arg.Do<SendSmtpEmail>(e => emailSent = e))
                .Returns(Task.FromResult(new CreateSmtpEmail { MessageId = "test-message-id" }));

            await sut.NotifyNewUserRegistered("Anders Hellman", "anders.hellman@arasaka.nc", "Night City");

            var params_ = emailSent.Params as Dictionary<string, object>;
            emailSent.TemplateId.Should().Be(1);
            emailSent.Sender.Email.Should().Be("server@arasaka.nc");
            emailSent.Sender.Name.Should().Be("Test System");
            emailSent.To.Should().HaveCount(1);
            emailSent.To.First().Email.Should().Be("admin@arasaka.nc");
            params_.Should().ContainKey("UserName");
            params_["UserName"].Should().Be("Anders Hellman");
            params_["UserEmail"].Should().Be("anders.hellman@arasaka.nc");
            params_["UserCity"].Should().Be("Night City");
            params_["Date"].Should().Be(DateTime.Now.ToString("dd.MM.yyyy"));
            params_["Time"].Should().Be(DateTime.Now.ToString("HH:mm"));
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
            SendSmtpEmail emailSent = null;
            
            var sut = CreateSut(out var brevoClient, out var logger);
            brevoClient.SendTransacEmailAsync(Arg.Do<SendSmtpEmail>(e => emailSent = e))
                .Returns(Task.FromResult(new CreateSmtpEmail { MessageId = "test-message-id" }));

            await sut.RemindOrganizationOnDispatchMonth(organization);

            var params_ = emailSent.Params as Dictionary<string, object>;
            emailSent.TemplateId.Should().Be(2);
            emailSent.Sender.Email.Should().Be("server@arasaka.nc");
            emailSent.Sender.Name.Should().Be("Test System");
            emailSent.To.Should().HaveCount(3);
            emailSent.To.Select(t => t.Email).Should().Contain("admin@arasaka.nc");
            emailSent.To.Select(t => t.Email).Should().Contain("member1@org.de");
            emailSent.To.Select(t => t.Email).Should().Contain("member2@org.de");
            params_.Should().ContainKey("OrganizationName");
            params_["OrganizationName"].Should().Be("Org 1");
            params_["DispatchMonth"].Should().Be("Oktober");
            params_["LatestCryoDate"].Should().Be("13.10.2024");
            logger.Messages[LogLevel.Information].Should().NotBeEmpty();
        }

        [Test]
        public async Task WhenBrevoNotificationFails_ErrorIsLogged()
        {
            var sut = CreateSut(out var brevoClient, out var logger);
            var apiException = new ApiException(400, "Bad Request");
            brevoClient.SendTransacEmailAsync(Arg.Any<SendSmtpEmail>())
                .Returns<CreateSmtpEmail>(_ => throw apiException);

            Assert.ThrowsAsync<ApiException>(async () => 
                await sut.NotifyNewUserRegistered("荒坂 三郎", "saburo.arasaka@arasaka.nc", "Night City").ConfigureAwait(true));

            logger.Messages[LogLevel.Error].Should().Contain("Bad Request");
        }

        private EmailNotificationService CreateSut(out IBrevoEmailClient brevoClient, out TestLogger<EmailNotificationService> logger)
        {
            brevoClient = Substitute.For<IBrevoEmailClient>();
            logger = new TestLogger<EmailNotificationService>();
            return new EmailNotificationService(brevoClient, logger, Options.Create(new ApplicationSettings {Application = new Application
            {
                SenderEmail = "server@arasaka.nc",
                SenderName = "Test System",
                AdministratorEmail = "admin@arasaka.nc",
                NewUserRegisteredTemplateId = 1,
                RemindOrganizationOnDispatchMonthTemplateId = 2
            }}));
        }
    }
}