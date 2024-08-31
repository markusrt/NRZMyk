using NRZMyk.Services.Data.Entities;
using System.Globalization;
using System;
using System.Threading.Tasks;
using Humanizer;

namespace NRZMyk.Services.Services;

public class ReminderService : IReminderService
{
    public string HumanReadableExpectedNextSending(Organization organization)
    {
        var expectedNextSending = CalculateExpectedNextSending(organization);

        if (expectedNextSending == null)
        {
            return "Kein Einsendemonat festgelegt";
        }

        var isExpectedThisMonth = expectedNextSending.Value.Month == DateTime.Today.Month &&
                                  expectedNextSending.Value.Year == DateTime.Today.Year;

        return isExpectedThisMonth
            ? "diesen Monat"
            : expectedNextSending.Humanize(culture: CultureInfo.GetCultureInfo("de"));
    }


    public DateTime? CalculateExpectedNextSending(Organization organization)
    {
        if (organization.DispatchMonth == MonthToDispatch.None || organization.LatestCryoDate == null)
        {
            return null;
        }

        var today = DateTime.Today;
        var expectedArrival = new DateTime(today.Year, (int)organization.DispatchMonth, 15);
        var timeSinceLastArrival = expectedArrival.Subtract(organization.LatestCryoDate.Value);

        if (timeSinceLastArrival.TotalDays < 0)
        {
            expectedArrival = new DateTime(today.Year+1, (int)organization.DispatchMonth, 15);
            timeSinceLastArrival = expectedArrival.Subtract(organization.LatestCryoDate.Value);
        }

        if (timeSinceLastArrival.TotalDays < 365)
        {
            if (organization.DispatchMonth == (MonthToDispatch)today.Month)
            {
                return new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            }
        }
        else
        {
            expectedArrival = new DateTime(Math.Min(today.Year - 1, organization.LatestCryoDate.Value.Year),
                (int)organization.DispatchMonth, 21);
        }

        return expectedArrival;
    }
}