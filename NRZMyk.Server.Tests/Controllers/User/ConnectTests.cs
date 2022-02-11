using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Mocks.TestUtils;
using NRZMyk.Server.Controllers.Account;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Specifications;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Server.Tests.Controllers.User
{
    public class ConnectTests
    {
        [Test]
        public async Task WhenConnectedWithoutAssignedOrganization_ReturnsNewAuthenticatedUserAsGuest()
        {
            var user = new ClaimsPrincipal();
            var sut = CreateSut(user, out var repository, out var mapper, out _);
            var mappedAccount = new RemoteAccount {DisplayName = "Jane Doe"};
            var storedAccount = new RemoteAccount {Id = 123, DisplayName = "Jane Doe"};
            mapper.Map<RemoteAccount>(user).Returns(mappedAccount);
            repository.AddAsync(mappedAccount).Returns(storedAccount);

            var action = await sut.HandleAsync().ConfigureAwait(true);

            mapper.Received(1).Map<RemoteAccount>(user);
            await repository.Received(1).AddAsync(mappedAccount).ConfigureAwait(true);
            var connectedAccount = action.Value.Should().BeOfType<ConnectedAccount>().Subject;
            connectedAccount.Account.Should().Be(storedAccount);
            connectedAccount.IsGuest.Should().BeTrue();
        }

        [Test]
        public async Task WhenConnectedWithoutAssignedOrganization_SendsEmailNotification()
        {
            var user = new ClaimsPrincipal();
            var sut = CreateSut(user, out var repository, out var mapper, out var emailNotificationService);
            var mappedAccount = new RemoteAccount { DisplayName = "Jane Doe", City = "New York", Email = "jane.doe@doyouknowthedoes.com"};
            var storedAccount = new RemoteAccount { Id = 123, DisplayName = "Jane Doe", City = "New York", Email = "jane.doe@doyouknowthedoes.com" };
            mapper.Map<RemoteAccount>(user).Returns(mappedAccount);
            repository.AddAsync(mappedAccount).Returns(storedAccount);

            await sut.HandleAsync().ConfigureAwait(true);

            await emailNotificationService.Received(1)
                .NotifyNewUserRegistered("Jane Doe", "jane.doe@doyouknowthedoes.com", "New York");
        }


        [Test]
        public async Task WhenConnectedWithAssignedOrganization_ReturnsNewAuthenticatedUserAsUser()
        {
            var user = new ClaimsPrincipal();
            var sut = CreateSut(user, out var repository, out var mapper, out _);
            var mappedAccount = new RemoteAccount {DisplayName = "Jane Doe"};
            var storedAccount = new RemoteAccount {Id = 123, DisplayName = "Jane Doe", OrganizationId = 12};
            mapper.Map<RemoteAccount>(user).Returns(mappedAccount);
            repository.AddAsync(mappedAccount).Returns(storedAccount);

            var action = await sut.HandleAsync().ConfigureAwait(true);

            mapper.Received(1).Map<RemoteAccount>(user);
            await repository.Received(1).AddAsync(mappedAccount).ConfigureAwait(true);
            var connectedAccount = action.Value.Should().BeOfType<ConnectedAccount>().Subject;
            connectedAccount.Account.Should().Be(storedAccount);
            connectedAccount.IsGuest.Should().BeFalse();
        }
        [Test]
        public async Task WhenConnected_UpdatesExistingUserDetails()
        {
            var user = new ClaimsPrincipal();
            var sut = CreateSut(user, out var repository, out var mapper, out _);
            var mappedAccount = new RemoteAccount {DisplayName = "Jane Doe", Street = "Long Road 1000233", ObjectId = Guid.NewGuid()};
            var existingAccount = new RemoteAccount {Id = 123, DisplayName = "Jane Doe"};
            mapper.Map<RemoteAccount>(user).Returns(mappedAccount);
            repository.FirstOrDefaultAsync(Arg.Is<RemoteAccountByObjectIdSpecification>(s => s.ObjectId == mappedAccount.ObjectId)).Returns(existingAccount);
            mapper.When(m => m.Map(user, existingAccount)).Do(info => info.Arg<RemoteAccount>().Street = "Long Road 1000233");

            var action = await sut.HandleAsync().ConfigureAwait(true);

            mapper.Received(1).Map(user, existingAccount);
            await repository.Received(1).UpdateAsync(existingAccount).ConfigureAwait(true);
            var connectedAccount = action.Value.Should().BeOfType<ConnectedAccount>().Subject;
            connectedAccount.Account.Should().Be(existingAccount);
            connectedAccount.IsGuest.Should().BeTrue();
        }

        private static Connect CreateSut(ClaimsPrincipal user,
            out IAsyncRepository<RemoteAccount> accountRepository, out IMapper map, out IEmailNotificationService emailNotificationService)
        {
            accountRepository = Substitute.For<IAsyncRepository<RemoteAccount>>();
            map = Substitute.For<IMapper>();
            emailNotificationService = Substitute.For<IEmailNotificationService>();
            return new Connect(accountRepository, map, emailNotificationService)
            {
                ControllerContext = new MockControllerContext("/api/connect/user", user:user)
            };
        }
    }
}