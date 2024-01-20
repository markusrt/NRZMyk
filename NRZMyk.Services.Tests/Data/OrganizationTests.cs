using NRZMyk.Services.Data.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Tests.Data;

public class OrganizationTests
{
    [Test]
    public void CheckDataAndSendReminders_NoDataEntered_Last12Months_SendEmail()
    {
        // Arrange
        Organization org = new Organization();
        org.Email = "example@example.com";
        org.DispatchMonth = MonthToDispatch.January;
        org.LatestDataEntryDate = new DateTime(2022, 1, 1);
        org.LatestStrainArrivalDate = new DateTime(2022, 1, 15);

        var emailService = Substitute.For<IEmailNotificationService>();

        // Act
        org.CheckDataAndSendReminders(emailService);

        // Assert
        emailService.Received(1).SendEmail("example@example.com", "No data was entered during the last 12 months.");
    }

    [Test]
    public void CheckDataAndSendReminders_DataEntered_NoStrainArrived_SendEmail()
    {
        // Arrange
        Organization org = new Organization();
        org.Email = "example@example.com";
        org.DispatchMonth = MonthToDispatch.January;
        org.LatestDataEntryDate = new DateTime(2023, 1, 5);
        org.LatestStrainArrivalDate = new DateTime(2022, 12, 20);

        var emailService = Substitute.For<IEmailNotificationService>();

        // Act
        org.CheckDataAndSendReminders(emailService);

        // Assert
        emailService.Received(1).SendEmail("example@example.com", "Data was entered, but no strain has arrived yet.");
    }
}