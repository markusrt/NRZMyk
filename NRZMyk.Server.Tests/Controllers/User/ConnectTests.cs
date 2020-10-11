using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Server.Controllers.User;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Controllers.User
{
    public class ConnectTests
    {
        [Test]
        public async Task WhenConnected_ReturnsNewAuthenticatedUserAsGuest()
        {
            var user = new ClaimsPrincipal();
            var sut = CreateSut(user, out var repository, out var mapper);
            var mappedAccount = new RemoteAccount {DisplayName = "Jane Doe"};
            var storedAccount = new RemoteAccount {Id = 123, DisplayName = "Jane Doe"};
            mapper.Map<RemoteAccount>(user).Returns(mappedAccount);
            repository.AddAsync(mappedAccount).Returns(storedAccount);

            var action = await sut.HandleAsync();

            mapper.Received(1).Map<RemoteAccount>(user);
            await repository.Received(1).AddAsync(mappedAccount);
            var connectedAccount = action.Value.Should().BeOfType<ConnectedAccount>().Subject;
            connectedAccount.Account.Should().Be(storedAccount);
            connectedAccount.IsGuest.Should().BeTrue();
        }

        [Test]
        public async Task WhenConnected_UpdatesExistingUserDetails()
        {
            var user = new ClaimsPrincipal();
            var sut = CreateSut(user, out var repository, out var mapper);
            var mappedAccount = new RemoteAccount {DisplayName = "Jane Doe", Street = "Long Road 1000233", ObjectId = Guid.NewGuid()};
            var existingAccount = new RemoteAccount {Id = 123, DisplayName = "Jane Doe"};
            mapper.Map<RemoteAccount>(user).Returns(mappedAccount);
            repository.FirstOrDefaultAsync(Arg.Is<RemoteAccountByObjectIdSpecification>(s => s.ObjectId == mappedAccount.ObjectId)).Returns(existingAccount);
            mapper.When(m => m.Map(user, existingAccount)).Do(info => info.Arg<RemoteAccount>().Street = "Long Road 1000233");

            var action = await sut.HandleAsync();

            mapper.Received(1).Map(user, existingAccount);
            await repository.Received(1).UpdateAsync(existingAccount);
            var connectedAccount = action.Value.Should().BeOfType<ConnectedAccount>().Subject;
            connectedAccount.Account.Should().Be(existingAccount);
            connectedAccount.IsGuest.Should().BeTrue();
        }

        private static Connect CreateSut(ClaimsPrincipal user,
            out IAsyncRepository<RemoteAccount> accountRepository, out IMapper map)
        {
            accountRepository = Substitute.For<IAsyncRepository<RemoteAccount>>();
            map = Substitute.For<IMapper>();
            var httpContext = new DefaultHttpContext {User = user};
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Scheme = "http";
            httpContext.Request.Path = new PathString("/api/connect/user");
            return new Connect(accountRepository, map)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }
    }
}