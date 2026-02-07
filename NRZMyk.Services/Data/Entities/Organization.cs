using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;
using Humanizer;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Data.Entities
{
    public class Organization : BaseEntity, IAggregateRoot
    {
        public string Name { get; set; }

        [JsonIgnore] public ICollection<RemoteAccount> Members { get; set; }

        public MonthToDispatch DispatchMonth { get; set; }

        public DateTime? LastReminderSent { get; set; }

        [NotMapped]
        public DateTime? LatestSamplingDate { get; set; }

        [NotMapped]
        public DateTime? LatestCryoDate { get; set; }

        [NotMapped]
        public int TotalCreatedNotStoredCount { get; set; }

        [NotMapped]
        public int TotalCryoArchivedCount { get; set; }

        [NotMapped]
        [JsonPropertyName("currentPeriodCreatedNotStoredCount")]
        public int CurrentYearCreatedNotStoredCount { get; set; }

        [NotMapped]
        [JsonPropertyName("currentPeriodCryoArchivedCount")]
        public int CurrentYearCryoArchivedCount { get; set; }
    }
}