using NRZMyk.Services.Data.Entities;
using System.Globalization;
using System;
using System.Threading.Tasks;
using Humanizer;

namespace NRZMyk.Services.Services;

public class ReminderService(TimeProvider timeProvider) : IReminderService
{
    public string HumanReadableExpectedNextSending(Organization organization)
    {
        var expectedNextSending = CalculateExpectedNextSending(organization);

        if (expectedNextSending == null)
        {
            return "Kein Einsendemonat festgelegt";
        }

        var today = timeProvider.GetLocalNow().DateTime;
        var isExpectedThisMonth = expectedNextSending.Value.Month == today.Month &&
                                  expectedNextSending.Value.Year == today.Year;

        return isExpectedThisMonth
            ? "diesen Monat"
            : expectedNextSending.Humanize(culture: CultureInfo.GetCultureInfo("de"), dateToCompareAgainst: today);
    }


    public DateTime? CalculateExpectedNextSending(Organization organization)
    {
        if (organization.DispatchMonth == MonthToDispatch.None)
        {
            return null;
        }

        var today = timeProvider.GetLocalNow().DateTime;
        var dispatchMonth = (int)organization.DispatchMonth;
        var expectedYear = today.Year;
        var expectedArrival = new DateTime(expectedYear, dispatchMonth, 1);

        if (organization.LatestCryoDate == null)
        {
            return expectedArrival;
        }

        var latestCryoDate = organization.LatestCryoDate.Value;
        var timeSinceLastArrival = expectedArrival.Subtract(latestCryoDate);
        var isOffByNoMoreThen40Days = timeSinceLastArrival.TotalDays is > -40 and < 40;
        var isSentAlreadyThisYear = expectedArrival.Year == latestCryoDate.Year;
        
        if (isOffByNoMoreThen40Days || isSentAlreadyThisYear)
        {
            expectedArrival = new DateTime(today.AddYears(1).Year, dispatchMonth, 1);
        }
        else if (timeSinceLastArrival.TotalDays > 365)
        {
            var expectedLastTime = new DateTime(latestCryoDate.Year, dispatchMonth, 1);
            var wasSentTheCorrectYearLastTime = expectedLastTime.Year == latestCryoDate.Year;
            expectedArrival = wasSentTheCorrectYearLastTime 
                ? new DateTime(latestCryoDate.AddYears(1).Year, dispatchMonth, 1)
                : expectedLastTime;
        }

        return expectedArrival;
    }
}