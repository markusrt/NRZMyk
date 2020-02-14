using FluentAssertions;
using NRZMyk.Components.Shared;
using NUnit.Framework;

namespace NRZMyk.ComponentsTests.Shared
{
    class NavMenuBaseTests
    {
        [Test]
        public void WhenCreated_ThenMenuIsCollapsed()
        {
            var sut = new NavMenuBase();

            sut.NavMenuCssClass.Should().Be("collapse");
        }

        [Test]
        public void WhenToggle_ThenMenuCssClassChanges()
        {
            var sut = new NavMenuBase();

            sut.ToggleNavMenu();

            sut.NavMenuCssClass.Should().BeNull();
        }
    }
}
