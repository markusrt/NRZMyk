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

    [TestCase(1, 6, "in 5 Monaten")]
    [TestCase(2, 6, "in 5 Monaten")]
    [TestCase(3, 6, "in 5 Monaten")]
    [TestCase(4, 6, "in 5 Monaten")]
    [TestCase(5, 6, "in 5 Monaten")]
    [TestCase(6, 6, "in 5 Monaten")]
    [TestCase(7, 6, "in 5 Monaten")]
    [TestCase(8, 6, "in 5 Monaten")]
    [TestCase(19, 6, "vor 6 Monaten")]
    [TestCase(44, 6, "vor 2 Jahren")]
    [TestCase(10, 2, "in einem Monat")]
    public void WhenExpectedNextSendingIsChecked_HumanReadableExpectedNextSendingShowsValidInformation(int monthSinceLatestStrainArrival, int monthUntilNextArrival, string expectedNextSending)
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        var latestCryoDate = DateTime.Today.AddMonths(-1 * monthSinceLatestStrainArrival).AddMonths(1);
        var expectedNextArrival = DateTime.Today.AddMonths(monthUntilNextArrival);
        expectedNextArrival = new DateTime(expectedNextArrival.Year, expectedNextArrival.Month,
            DateTime.DaysInMonth(expectedNextArrival.Year, expectedNextArrival.Month));
        org.DispatchMonth = (MonthToDispatch)expectedNextArrival.Month;
        org.LatestCryoDate = new DateTime(latestCryoDate.Year, latestCryoDate.Month, 5);

        sut.HumanReadableExpectedNextSending(org).Should().Be(expectedNextSending);
    }

    [Test]
    public void WhenExpectedNextSendingIsChecked_SentSlightlyEarlyShowsDueNextYear()
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        var today = DateTime.Today;
        var twentyDaysAgo = today.Subtract(TimeSpan.FromDays(20));
        org.DispatchMonth = (MonthToDispatch) today.Month;
        org.LatestCryoDate = twentyDaysAgo;

        sut.HumanReadableExpectedNextSending(org).Should().Be("in einem Jahr");
    }

    [Test]
    public void WhenExpectedNextSendingIsChecked_SentSlightlyLateShowsDueNextYear()
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        var today = DateTime.Today;
        var lastMonth = DateTime.Today.AddMonths(-1);
        var someDaysAgo = today.AddDays(-20);
        org.DispatchMonth = (MonthToDispatch) lastMonth.Month;
        org.LatestCryoDate = someDaysAgo;

        sut.HumanReadableExpectedNextSending(org).Should().MatchRegex("in 1[01] Monaten");
    }

    [Test]
    public void WhenExpectedNextSendingIsChecked_SentTooLateShowsDueThisYear()
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        var today = DateTime.Today;
        var threeMonthAgo = DateTime.Today.AddMonths(-3);
        var moreThenAYearAgo = today.AddDays(-20).AddYears(-1);
        org.DispatchMonth = (MonthToDispatch) threeMonthAgo.Month;
        org.LatestCryoDate = moreThenAYearAgo;

        sut.HumanReadableExpectedNextSending(org).Should().MatchRegex("vor 3 Monaten");
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

    [Test]
    public void WhenLatestCryoDateIsNull_CalculateExpectedNextSendingReturnsNull()
    {
        var sut = CreateSut();
        var org = CreateOrganization();
        var today = DateTime.Today.AddMonths(2);
        org.DispatchMonth = (MonthToDispatch) today.Month;
        org.LatestCryoDate = null;

        sut.CalculateExpectedNextSending(org).Should().NotBeNull();
        sut.HumanReadableExpectedNextSending(org).Should().Be(today.Day == 1 ? "in 2 Monaten" : "in einem Monat");
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

    private static ReminderService CreateSut()
    {
        return new ReminderService();
    }
}