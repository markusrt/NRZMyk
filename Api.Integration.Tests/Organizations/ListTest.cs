using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NRZMyk.Services.Data;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NUnit.Framework;
using PublicApiIntegrationTests;
using Organization = NRZMyk.Services.Data.Entities.Organization;

namespace Api.Integration.Tests.Organizations
{
    public class ListTest
    {
        [OneTimeSetUp]
        protected void SeedData()
        {
            using (var scope = ClientFactory.ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                db.Organizations.Add(new Organization
                    { Name = "Test", DispatchMonth = MonthToDispatch.October });
                db.SaveChanges();
            }
        }

        [Test]
        public async Task WhenCreatingValidSentinelEntry_RespondsWithCreate()
        {
            var client = ClientFactory.CreateClient();

            var organizations = await client.GetFromJsonAsync<List<Organization>>("api/organizations").ConfigureAwait(true);
            
            organizations.Should().Contain(org => org.Name == "Test");
        }
    }
}
