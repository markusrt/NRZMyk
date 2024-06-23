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
    }
}