using FluentAssertions;
using NRZMyk.Components.Pages;
using NUnit.Framework;

namespace NRZMyk.ComponentsTests.Pages
{
    public class CounterBaseTests
    {
        [Test]
        public void WhenCreated_ThenCountIsZero()
        {
            var sut = new CounterBase();

            sut.CurrentCount.Should().Be(0);
        }

        [Test]
        public void WhenIncrementCount_ThenCountIsIncreased()
        {
            var sut = new CounterBase();

            sut.IncrementCount();

            sut.CurrentCount.Should().Be(1);
        }
    }
}
