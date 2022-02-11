using FluentAssertions;
using NRZMyk.Components.SharedComponents;
using NUnit.Framework;

namespace NRZMyk.ComponentsTests.SharedComponents
{
    public class NavMenuBaseTests
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

            sut.NavMenuCssClass.Should().BeEmpty();
        }
    }
}
