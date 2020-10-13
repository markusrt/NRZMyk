using System;
using Ardalis.Specification;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Specifications
{
    public class RemoteAccountByObjectIdSpecification : BaseSpecification<RemoteAccount>
    {
        public Guid ObjectId { get; set; }

        public RemoteAccountByObjectIdSpecification(Guid objectId) : base(a => a.ObjectId == objectId)
        {
            ObjectId = objectId;
        }
    }
}