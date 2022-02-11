using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Controllers.Account;
using NRZMyk.Server.Controllers.SentinelEntries;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;
using Tynamix.ObjectFiller;

namespace NRZMyk.Server.Tests.Controllers.SentinelEntries
{
    public class ListUsersTests
    {
        private Filler<RemoteAccount> _filler = new Filler<RemoteAccount>();

        [Test]
        public async Task WhenProtectKeyIsNegative_QueriesAllEntries()
        {
            var sut = CreateSut(out var repository, out var userService);
            var expectedResult = _filler.Create(2);
           
            repository.ListAllAsync()
                .Returns(Task.FromResult((IReadOnlyList<RemoteAccount>)expectedResult));

            var action = await sut.HandleAsync().ConfigureAwait(true);

            action.Result.Should().BeOfType<OkObjectResult>();
            action.Result.As<OkObjectResult>().Value.Should().Be(expectedResult);
            await userService.Received(1).GetRolesViaGraphApi(expectedResult).ConfigureAwait(true);
        }


        private static ListUsers CreateSut(out IAsyncRepository<RemoteAccount> repository, out IUserService userService)
        {
            repository = Substitute.For<IAsyncRepository<RemoteAccount>>();
            userService = Substitute.For<IUserService>();
            
            return new ListUsers(repository, userService)
            {
                ControllerContext = new MockControllerContext()
            };
        }
    }
}