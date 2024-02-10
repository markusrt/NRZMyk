using System;
using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public sealed class RemoteAccountByObjectIdSpecification : Specification<RemoteAccount>
    {
        public Guid ObjectId { get; set; }

        public RemoteAccountByObjectIdSpecification(Guid objectId)
        {
            ObjectId = objectId;
            Query.Where(a => a.ObjectId == objectId);
        }
    }
}