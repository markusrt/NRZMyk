using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using Tynamix.ObjectFiller;

namespace NRZMyk.Mocks.MockServices
{
    public class MockAccountService : IAccountService
    {
        private List<RemoteAccount> _accounts = new List<RemoteAccount>();

        private List<Organization> _organization = new List<Organization>();

        public MockAccountService()
        {
            var filler = new Filler<RemoteAccount>();
            _accounts.AddRange(filler.Create(10));

            _organization.Add(new Organization
            {
                Id = 1,
                Name = "Organization 1"
            });
            _organization.Add(new Organization
            {
                Id = 2,
                Name = "Organization 2"
            });
        }

        public async Task<ICollection<RemoteAccount>> ListAccounts()
        {
            await Task.Delay(2000);
            return _accounts.ToList();
        }

        public async Task<ICollection<Organization>> ListOrganizations()
        {
            await Task.Delay(2000);
            return _organization;
        }

        public async Task AssignToOrganizationAsync(ICollection<RemoteAccount> accounts)
        {
            await Task.Delay(1000);
            _accounts.Clear();
            _accounts.AddRange(accounts);
        }
    }
}