using NRZMyk.Services.Data.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Humanizer;
using Microsoft.Extensions.Time.Testing;
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
        org.LatestCryoDate = today.Subtract(TimeSpan.FromDays(370));

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
    [TestCase(44, 6, "vor 3 Jahren")]
    [TestCase(10, 2, "in einem Monat")]
    public void WhenExpectedNextSendingIsChecked_HumanReadableExpectedNextSendingShowsValidInformation(int monthSinceLatestStrainArrival, int monthUntilNextArrival, string expectedNextSending)
    {
        var fakeToday = new DateTime(2021, 7, 15);
        var fakeTimeProvider = new FakeTimeProvider(fakeToday);
        var sut = CreateSut(fakeTimeProvider);
        var org = CreateOrganization();
        var latestCryoDate = fakeToday.AddMonths(-1 * monthSinceLatestStrainArrival).AddMonths(1);
        var expectedNextArrival = fakeToday.AddMonths(monthUntilNextArrival);
        expectedNextArrival = new DateTime(expectedNextArrival.Year, expectedNextArrival.Month,
            DateTime.DaysInMonth(expectedNextArrival.Year, expectedNextArrival.Month));
        org.DispatchMonth = (MonthToDispatch)expectedNextArrival.Month;
        org.LatestCryoDate = new DateTime(latestCryoDate.Year, latestCryoDate.Month, 5);

        sut.HumanReadableExpectedNextSending(org).Should().Be(expectedNextSending);
    }

    [Test]
    public void WhenExpectedNextSendingIsChecked_SentSlightlyEarlyShowsDueNextYear()
    {
        var fakeToday = new DateTime(2021, 7, 15);
        var fakeTimeProvider = new FakeTimeProvider(fakeToday);
        var sut = CreateSut(fakeTimeProvider);
        var org = CreateOrganization();
        var twentyDaysAgo = fakeToday.Subtract(TimeSpan.FromDays(20));
        org.DispatchMonth = (MonthToDispatch) fakeToday.Month;
        org.LatestCryoDate = twentyDaysAgo;

        sut.HumanReadableExpectedNextSending(org).Should().MatchRegex("in einem Jahr|in 1[01] Monaten");
    }

    [Test]
    public void WhenExpectedNextSendingIsChecked_SentSlightlyLateShowsDueNextYear()
    {
        var fakeToday = new DateTime(2021, 7, 15);
        var fakeTimeProvider = new FakeTimeProvider(fakeToday);
        var sut = CreateSut(fakeTimeProvider);
        var org = CreateOrganization();
        var lastMonth = fakeToday.AddMonths(-1);
        var someDaysAgo = fakeToday.AddDays(-20);
        org.DispatchMonth = (MonthToDispatch) lastMonth.Month;
        org.LatestCryoDate = someDaysAgo;

        sut.HumanReadableExpectedNextSending(org).Should().MatchRegex("in einem Jahr|in 1[01] Monaten");
    }

    [Test]
    public void WhenExpectedNextSendingIsChecked_SentSlightlyEarlyLastTimeShowsDueTheYearAfter()
    {
        var fakeToday = new DateTime(2021, 8, 15);
        var fakeTimeProvider = new FakeTimeProvider(fakeToday);
        var sut = CreateSut(fakeTimeProvider);
        var org = CreateOrganization();

        var december2020 = new DateTime(2020, 12, 25);
        org.DispatchMonth = MonthToDispatch.January;
        org.LatestCryoDate = december2020;

        sut.HumanReadableExpectedNextSending(org).Should().Be("in 4 Monaten");
    }

    [Test]
    public void WhenExpectedNextSendingIsChecked_SentTooEarlyButOnTheCorrectYearShowsDueTheYearAfter()
    {
        var fakeToday = new DateTime(2021, 7, 15);
        var fakeTimeProvider = new FakeTimeProvider(fakeToday);
        var sut = CreateSut(fakeTimeProvider);
        var org = CreateOrganization();

        var march2021 = new DateTime(2021, 3, 20);
        org.DispatchMonth = MonthToDispatch.July;
        org.LatestCryoDate = march2021;

        sut.CalculateExpectedNextSending(org).Should().Be(new DateTime(2022, 7, 1));
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
        var fakeToday = new DateTime(2021, 7, 15);
        var fakeTimeProvider = new FakeTimeProvider(fakeToday);
        var sut = CreateSut(fakeTimeProvider);
        var org = CreateOrganization();
        var futureDate = fakeToday.AddMonths(2);
        org.DispatchMonth = (MonthToDispatch) futureDate.Month;
        org.LatestCryoDate = null;

        sut.CalculateExpectedNextSending(org).Should().NotBeNull();
        sut.HumanReadableExpectedNextSending(org).Should().Be("in einem Monat");
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

    private static ReminderService CreateSut(TimeProvider timeProvider = null)
    {
        return new ReminderService(timeProvider ?? TimeProvider.System);
    }
}