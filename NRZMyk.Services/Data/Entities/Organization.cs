using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using Humanizer;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;

namespace NRZMyk.Services.Data.Entities
{
    public class Organization : BaseEntity, IAggregateRoot
    {
        public string Name { get; set; }

        [JsonIgnore] public ICollection<RemoteAccount> Members { get; set; }

        public string Email { get; set; }
        public MonthToDispatch DispatchMonth { get; set; }
        public DateTime LatestDataEntryDate { get; set; }
        public DateTime LatestStrainArrivalDate { get; set; }

        [Display(Name = "Nächste Einsendung erwarted")]
        public string ExpectedNextSending => CalculateExpectedNextSending();

        private string CalculateExpectedNextSending()
        {
            var today = DateTime.Today;
            var expectedArrival = new DateTime(today.Year, (int)DispatchMonth, 15);
            var timeSinceLastArrival = expectedArrival.Subtract(LatestStrainArrivalDate);

            if (timeSinceLastArrival.TotalDays < 365)
            {
                if (DispatchMonth == (MonthToDispatch)today.Month)
                {
                    return "diesen Monat";
                }
            }
            else
            {
                expectedArrival = new DateTime(Math.Min(today.Year - 1, LatestStrainArrivalDate.Year), (int)DispatchMonth, 21);
            }

            return expectedArrival.Humanize(culture: CultureInfo.GetCultureInfo("de-de"));
        }

        public void CheckDataAndSendReminders(IEmailNotificationService emailService)
        {
            var currentDate = DateTime.Now;
            var lastDispatchMonth = new DateTime(currentDate.Year, (int)DispatchMonth, 1);
            var nextDispatchMonth = lastDispatchMonth.AddMonths(12);

            // Check if the current date is the first day of the dispatch month
            if (currentDate.Date == lastDispatchMonth.Date)
            {
                // Check if no data was entered during the last 12 months
                if (LatestDataEntryDate < lastDispatchMonth)
                {
                    string message = "No data was entered during the last 12 months.";
                    emailService.SendEmail(Email, message);
                }
            }
            // Check if the current date is the first day after the dispatch month ends
            else if (currentDate.Date == nextDispatchMonth.Date)
            {
                // Check if data was entered but no strain has arrived yet
                if (LatestDataEntryDate >= lastDispatchMonth && LatestStrainArrivalDate < lastDispatchMonth)
                {
                    string message = "Data was entered, but no strain has arrived yet.";
                    emailService.SendEmail(Email, message);
                }
            }
        }
    }
}