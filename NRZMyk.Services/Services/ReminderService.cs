using NRZMyk.Services.Data.Entities;
using System.Globalization;
using System;
using Humanizer;

namespace NRZMyk.Services.Services;

public class ReminderService : IReminderService
{
    private readonly IEmailNotificationService _emailNotificationService;

    public ReminderService(IEmailNotificationService emailNotificationService)
    {
        _emailNotificationService = emailNotificationService;
    }

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

    public void CheckDataAndSendReminders(Organization organization)
    {
        var expectedNextSending = CalculateExpectedNextSending(organization);
        if (DateTime.Today > expectedNextSending)
        {
            string message = "Data was entered, but no strain has arrived yet.";
            _emailNotificationService.SendEmail(organization.Email, message);
        }
    }
}