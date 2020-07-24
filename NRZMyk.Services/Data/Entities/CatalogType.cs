using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data.Entities
{
    public class CatalogType : BaseEntity, IAggregateRoot
    {
        public string Type { get; private set; }
        public CatalogType(string type)
        {
            Type = type;
        }
    }
}
