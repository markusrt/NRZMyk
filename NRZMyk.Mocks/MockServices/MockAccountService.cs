using System;
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
        public static int Delay = 2000;

        private static readonly Random Random = new();

        private static readonly List<Tuple<string, MonthToDispatch>> Organizations = new()
        {
            Tuple.Create("München (TU)", MonthToDispatch.January),
            Tuple.Create("Düsseldorf", MonthToDispatch.February),
            Tuple.Create("München (LMU)", MonthToDispatch.March),
            Tuple.Create("Essen", MonthToDispatch.April),
            Tuple.Create("Freiburg", MonthToDispatch.May),
            Tuple.Create("Berlin", MonthToDispatch.June),
            Tuple.Create("Oldenburg", MonthToDispatch.July),
            Tuple.Create("Frankfurt", MonthToDispatch.August),
            Tuple.Create("Erlangen", MonthToDispatch.September),
            Tuple.Create("Aachen", MonthToDispatch.October),
            Tuple.Create("Nürnberg", MonthToDispatch.November),
            Tuple.Create("Würzburg", MonthToDispatch.December)
        };

        private readonly List<RemoteAccount> _accounts = new();

        private readonly List<Organization> _organization = new();

        public MockAccountService()
        {
            var filler = new Filler<RemoteAccount>();
            _accounts.AddRange(filler.Create(10));


            int id = 1;
            foreach (var organization in Organizations)
            {
                var randomDayOffset = Random.Next(365 * 2);
                var latestStrainArrivalDate = DateTime.Today.AddDays(-1 * randomDayOffset);
                var latestDataEntryDate = latestStrainArrivalDate.AddDays(randomDayOffset/2.0);
                _organization.Add(new Organization
                {
                    Id = id++,
                    Name = organization.Item1,
                    DispatchMonth = organization.Item2,
                    LatestCryoDate = latestStrainArrivalDate,
                    LatestSamplingDate = latestDataEntryDate,
                    TotalCreatedNotStoredCount = Random.Next(0, 50),
                    TotalCryoArchivedCount = Random.Next(50, 200),
                    CurrentPeriodCreatedNotStoredCount = Random.Next(0, 20),
                    CurrentPeriodCryoArchivedCount = Random.Next(0, 50)
                });
            }

            foreach (var account in _accounts)
            {
                var organization = _organization[Random.Next(Organizations.Count)];
                account.Organization = organization;
                account.OrganizationId = organization.Id;
            }
        }

        public async Task<ICollection<RemoteAccount>> ListAccounts()
        {
            await Task.Delay(Delay);
            return _accounts.ToList();
        }

        public async Task<ICollection<Organization>> ListOrganizations()
        {
            await Task.Delay(Delay);
            return _organization;
        }

        public async Task<int> AssignToOrganization(ICollection<RemoteAccount> accounts)
        {
            await Task.Delay(Delay);
            _accounts.Clear();
            _accounts.AddRange(accounts);
            return accounts.Count;
        }
    }
}