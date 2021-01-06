using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Services
{
    public class ProtectKeyToOrganizationResolverTests
    {
        [Test]
        public void Ctor_DoesNotThrow()
        {
            var sut = CreateSut(out _);

            sut.Should().NotBeNull();
        }

        [Test]
        public async Task WhenOrganizationExists_NameIsResolved()
        {
            var sut = CreateSut(out var repository);
            repository.GetByIdAsync(10).Returns(new Organization {Name = "Laboratory 1"});

            var organization = await sut.ResolveOrganization("10");

            organization.Should().Be("Laboratory 1");
        }

        [Test]
        public async Task WhenOrganizationExists_CacheIsFilled()
        {
            var sut = CreateSut(out var repository);
            repository.GetByIdAsync(10).Returns(new Organization {Name = "Laboratory 1"});
            _ = await sut.ResolveOrganization("10");
            repository.GetByIdAsync(Arg.Any<int>()).Throws(new Exception());

            var organization = await sut.ResolveOrganization("10");

            organization.Should().Be("Laboratory 1");
        }

        [Test]
        public async Task WhenOrganizationKeyIsInvalid_EmptyStringIsReturned()
        {
            var sut = CreateSut(out _);

            var organization = await sut.ResolveOrganization("Foo");

            organization.Should().BeEmpty();
        }

        [Test]
        public async Task WhenOrganizationIsNotFound_EmptyStringIsReturned()
        {
            var sut = CreateSut(out var repository);
            repository.GetByIdAsync(404).Returns((Organization)null);

            var organization = await sut.ResolveOrganization("404");

            organization.Should().BeEmpty();
        }

        private ProtectKeyToOrganizationResolver CreateSut(out IAsyncRepository<Organization> repository)
        {
            repository = Substitute.For<IAsyncRepository<Organization>>();
            return new ProtectKeyToOrganizationResolver(Substitute.For<ILogger<ProtectKeyToOrganizationResolver>>(), repository);
        }
    }
}