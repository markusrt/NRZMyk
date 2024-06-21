using NRZMyk.Services.Data.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FluentAssertions;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Tests.Data;

public class OrganizationTests
{
    [Test]
    public void CheckDataAndSendReminders_NoDataEntered_Last12Months_SendEmail()
    {
        var org = new Organization
        {
            Id = 0,
            Name = null,
            Members = null,
            Email = null,
            DispatchMonth = MonthToDispatch.None,
            LatestDataEntryDate = default,
            LatestStrainArrivalDate = default
        };
        org.Email = "example@example.com";
        org.DispatchMonth = MonthToDispatch.January;
        org.LatestDataEntryDate = new DateTime(2022, 1, 1);
        org.LatestStrainArrivalDate = new DateTime(2022, 1, 15);

        var emailService = Substitute.For<IEmailNotificationService>();

        org.CheckDataAndSendReminders(emailService);

        emailService.Received(1).SendEmail("example@example.com", "No data was entered during the last 12 months.");
    }

    [Test]
    public void CheckDataAndSendReminders_DataEntered_NoStrainArrived_SendEmail()
    {
        var org = CreateOrganization();
        org.DispatchMonth = MonthToDispatch.January;
        org.LatestDataEntryDate = new DateTime(2023, 1, 5);
        org.LatestStrainArrivalDate = new DateTime(2022, 12, 20);

        var emailService = Substitute.For<IEmailNotificationService>();

        org.CheckDataAndSendReminders(emailService);

        emailService.Received(1).SendEmail("example@example.com", "Data was entered, but no strain has arrived yet.");
    }

    [Test]
    public void WhenExpectedNextSendingIsThisMonth_ShowsHumanReadableInformation()
    {
        var org = CreateOrganization();
        var today = DateTime.Today;
        org.DispatchMonth = (MonthToDispatch)today.Month;
        org.LatestStrainArrivalDate = today.Subtract(TimeSpan.FromDays(200));

        org.ExpectedNextSending.Should().Be("diesen Monat");
    }

    [TestCase(1, 6, "in 5 Monaten")]
    [TestCase(2, 6, "in 5 Monaten")]
    [TestCase(3, 6, "in 5 Monaten")]
    [TestCase(4, 6, "in 5 Monaten")]
    [TestCase(5, 6, "in 5 Monaten")]
    [TestCase(6, 6, "in 5 Monaten")]
    [TestCase(7, 6, "vor 6 Monaten")]
    [TestCase(8, 6, "vor 6 Monaten")]
    [TestCase(18, 6, "vor einem Jahr")]
    [TestCase(48, 6, "vor 3 Jahren")]
    [TestCase(10, 2, "in einem Monat")]
    public void WhenExpectedNextSendingIsChecked_ShowsHumanReadableInformation(int monthSinceLatestStrainArrival, int monthUntilNextArrival, string expectedNextSending)
    {
        var org = CreateOrganization();
        var todayInSixMonths = DateTime.Today.AddMonths(monthUntilNextArrival);
        org.DispatchMonth = (MonthToDispatch)todayInSixMonths.Month;
        org.LatestStrainArrivalDate = DateTime.Today.AddMonths(-1*monthSinceLatestStrainArrival);

        org.ExpectedNextSending.Should().Be(expectedNextSending);
    }

    private static Organization CreateOrganization()
    {
        return new Organization
        {
            Id = 1,
            Name = "Example",
            Members = new List<RemoteAccount>(),
            Email = "example@example.com",
            DispatchMonth = MonthToDispatch.None,
        };
    }
}