using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class OrganizationsIncludingRemoteAccountSpecification : Specification<Organization>
    {
        public OrganizationsIncludingRemoteAccountSpecification()
        {
            Query
                .Include(o => o.Members);
        }
    }
}
