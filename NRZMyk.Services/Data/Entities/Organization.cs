using System.Collections.Generic;
using System.Text.Json.Serialization;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data.Entities
{
    public class Organization: BaseEntity, IAggregateRoot
    {
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<RemoteAccount> Members { get; set; }
    }
}