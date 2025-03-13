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
        if (organization.DispatchMonth == MonthToDispatch.None)
        {
            return null;
        }

        var today = DateTime.Today;
        var dispatchMonth = (int)organization.DispatchMonth;
        var expectedYear = today.Year;
        if (today.Month > dispatchMonth)
        {
            expectedYear++;
        }
        var expectedArrival = new DateTime(expectedYear, dispatchMonth, 1);

        if (organization.LatestCryoDate == null)
        {
            return expectedArrival;
        }

        var timeSinceLastArrival = expectedArrival.Subtract(organization.LatestCryoDate.Value);

        if (timeSinceLastArrival.TotalDays > -30 && timeSinceLastArrival.TotalDays < 30 )
        {
            expectedArrival = new DateTime(today.AddYears(1).Year, (int)organization.DispatchMonth, 1);
        }
        else if (timeSinceLastArrival.TotalDays > 365)
        {
            expectedArrival = new DateTime(organization.LatestCryoDate.Value.AddYears(1).Year, (int)organization.DispatchMonth, 1);
        }

        return expectedArrival;
    }
}