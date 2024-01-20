using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
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