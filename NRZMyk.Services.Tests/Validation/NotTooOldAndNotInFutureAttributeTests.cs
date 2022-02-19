using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using NRZMyk.Services.Validation;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Validation
{
    public class NotTooOldAndNotInFutureAttributeTests
    {
        [Test]
        public void WhenValueIsNull_IsTreatedAsValid()
        {
            var sut = CreateSut();
            DateTime? emptyDate = null;

            sut.IsValid(emptyDate).Should().BeTrue();
        }

        [Test]
        public void WhenValueIsNoDate_IsTreatedAsValid()
        {
            var sut = CreateSut();

            sut.IsValid("10.10.2000").Should().BeTrue();
        }

        [Test]
        public void WhenValueIsInFuture_IsTreatedAsInvalid()
        {
            var sut = CreateSut();
            var futureDate = DateTime.Now.AddDays(10);

            var validationContext = new ValidationContext(futureDate) {MemberName = "SamplingDate"};
            var result = sut.GetValidationResult(futureDate, validationContext);
            
            result.MemberNames.Should().BeEquivalentTo(new List<string>{"SamplingDate"});
            result.ErrorMessage.Should().NotBeEmpty();
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(100)]
        public void WhenValueIsOlderThenSpecifiedYears_IsTreatedAsInvalid(int maxAgeInYears)
        {
            var sut = CreateSut(maxAgeInYears);
            var futureDate = DateTime.Now.AddYears(-maxAgeInYears).AddHours(-1);

            var validationContext = new ValidationContext(futureDate) {MemberName = "SamplingDate"};
            var result = sut.GetValidationResult(futureDate, validationContext);
            
            result.MemberNames.Should().BeEquivalentTo(new List<string>{"SamplingDate"});
            result.ErrorMessage.Should().Contain(maxAgeInYears.ToString());
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void WhenMaxAgeInYearsIsLessOrEqualThenZero_ThrowsArgumentException(int maxAgeInYears)
        {
            Action createSut = () => _ = CreateSut(maxAgeInYears);

            createSut.Should().Throw<ArgumentException>();
        }
        
        private static NotTooOldAndNotInFutureAttribute CreateSut(int maxAgeInYears = 5)
        {
            return new NotTooOldAndNotInFutureAttribute(maxAgeInYears);
        }
        
    }
}