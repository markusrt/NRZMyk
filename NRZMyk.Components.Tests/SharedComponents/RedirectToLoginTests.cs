using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using NRZMyk.Components.SharedComponents;
using NUnit.Framework;
using TestContext = Bunit.TestContext;

namespace NRZMyk.Components.Tests.Pages
{
    public class RedirectToLoginTests
    {
        private TestContext _context;
        private IRenderedComponent<RedirectToLogin> _sut;

        [SetUp]
        public void CreateComponent()
        {
            _context = new TestContext();
        }

        [TearDown]
        public void DisposeContext()
        {
            _sut.Dispose();
            _context.Dispose();
        }

        [Test]
        public async Task WhenInitialized_RedirectsToLogin()
        {
            var navigationManager = _context.Services.GetRequiredService<FakeNavigationManager>();
            navigationManager.NavigateTo("http://localhost/original-uri");

            _sut = CreateSut();

            var loginNavigation = navigationManager.History.First();
            loginNavigation.Uri.Should().Be("authentication/login");

            var requestOptions = loginNavigation.StateFromJson<InteractiveRequestOptions>();
            requestOptions.Should().NotBeNull();
            requestOptions.Interaction.Should().Be(InteractionType.SignIn);
            requestOptions.ReturnUrl.Should().Be("http://localhost/original-uri");
        }

        private IRenderedComponent<RedirectToLogin> CreateSut()
        {
            return _context.RenderComponent<RedirectToLogin>();
        }
    }
}