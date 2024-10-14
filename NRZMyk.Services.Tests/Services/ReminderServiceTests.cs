using NRZMyk.Services.Data.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FluentAssertions;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Tests.Services;

public class ReminderServiceTests
{


    [Test]
    public void WhenExpectedNextSendingIsThisMonth_HumanReadableExpectedNextSendingShowsValidInformation()
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        var today = DateTime.Today;
        org.DispatchMonth = (MonthToDispatch)today.Month;
        org.LatestCryoDate = today.Subtract(TimeSpan.FromDays(200));

        sut.HumanReadableExpectedNextSending(org).Should().Be("diesen Monat");
    }

    [TestCase(1, 6, "in 6 Monaten")]
    [TestCase(2, 6, "in 6 Monaten")]
    [TestCase(3, 6, "in 6 Monaten")]
    [TestCase(4, 6, "in 6 Monaten")]
    [TestCase(5, 6, "in 6 Monaten")]
    [TestCase(6, 6, "in 6 Monaten")]
    [TestCase(7, 6, "vor 6 Monaten")]
    [TestCase(8, 6, "vor 6 Monaten")]
    [TestCase(19, 6, "vor einem Jahr")]
    [TestCase(43, 6, "vor 3 Jahren")]
    [TestCase(10, 2, "in 2 Monaten")]
    public void WhenExpectedNextSendingIsChecked_HumanReadableExpectedNextSendingShowsValidInformation(int monthSinceLatestStrainArrival, int monthUntilNextArrival, string expectedNextSending)
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        var expectedNextArrival = DateTime.Today.AddMonths(monthUntilNextArrival);
        expectedNextArrival = new DateTime(expectedNextArrival.Year, expectedNextArrival.Month,
            DateTime.DaysInMonth(expectedNextArrival.Year, expectedNextArrival.Month));
        org.DispatchMonth = (MonthToDispatch)expectedNextArrival.Month;
        org.LatestCryoDate = DateTime.Today.AddMonths(-1 * monthSinceLatestStrainArrival).AddMonths(1);
        org.LatestCryoDate = new DateTime(org.LatestCryoDate.Value.Year, org.LatestCryoDate.Value.Month, 5);

        sut.HumanReadableExpectedNextSending(org).Should().Be(expectedNextSending);
    }

    [Test]
    public void WhenMonthToDispatchIsNone_HumanReadableExpectedNextSendingShowsNoDispatchMonthSet()
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        org.DispatchMonth = MonthToDispatch.None;
        org.LatestCryoDate = DateTime.MinValue;

        sut.HumanReadableExpectedNextSending(org).Should().Be("Kein Einsendemonat festgelegt");
    }

    [Test]
    public void WhenMonthToDispatchIsNone_CalculateExpectedNextSendingReturnsNull()
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        org.DispatchMonth = MonthToDispatch.None;
        org.LatestCryoDate = DateTime.MinValue;

        sut.CalculateExpectedNextSending(org).Should().BeNull();
    }

    private static Organization CreateOrganization()
    {
        return new Organization
        {
            Id = 1,
            Name = "Example",
            Members = new List<RemoteAccount>(),
            DispatchMonth = MonthToDispatch.None
        };
    }

    private static ReminderService CreateSut(IEmailNotificationService emailNotificationServiceMock = null)
    {
        return new ReminderService();
    }
}