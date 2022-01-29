using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Data.Entities
{
    public class RemoteAccount : BaseEntity, IAggregateRoot
    {
        public Guid ObjectId { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Postalcode { get; set; }

        public string Street { get; set; }

        public int? OrganizationId { get; set; }

        public Organization Organization { get; set; }

        [NotMapped]
        public Role Role { get; set; }
    }
}
