using System;
using System.Collections.Generic;
using System.Text;
using NRZMyk.Services.Interfaces;

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
    }
}
