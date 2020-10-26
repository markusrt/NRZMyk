using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using NRZMyk.Services.Services;
using NRZMyk.Services.Validation;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Validation
{
    public class SensitivityTestAttributeTests
    {
        [Test]
        public void WhenValueIsNull_IsTreatedAsValid()
        {
            var sut = CreateSut();
            List<AntimicrobialSensitivityTestRequest> sensitivityTests = null;

            sut.IsValid(sensitivityTests).Should().BeTrue();
        }

        [Test]
        public void WhenValueIsOtherType_IsTreatedAsValid()
        {
            var sut = CreateSut();
            var noSensitivityTests = new List<string> {"Foo"};

            sut.IsValid(noSensitivityTests).Should().BeTrue();
        }

        [Test]
        public void WhenValueIsContainingMicValue_IsTreatedAsValid()
        {
            var sut = CreateSut();
            var sensitivityTests = new List<AntimicrobialSensitivityTestRequest> {
                new AntimicrobialSensitivityTestRequest { MinimumInhibitoryConcentration = 0.12f },
                new AntimicrobialSensitivityTestRequest { MinimumInhibitoryConcentration = 0.25f }
            };

            sut.IsValid(sensitivityTests).Should().BeTrue();
        }

        
        [Test]
        public void WhenValueIsMissingMicValue_IsTreatedAsValid()
        {
            var sut = CreateSut();
            var sensitivityTests = new List<AntimicrobialSensitivityTestRequest> {
                new AntimicrobialSensitivityTestRequest { MinimumInhibitoryConcentration = null },
                new AntimicrobialSensitivityTestRequest { MinimumInhibitoryConcentration = 0.25f }
            };
            var validationContext = new ValidationContext(sensitivityTests) {MemberName = "ValidatedProperty"};
            var result = sut.GetValidationResult(sensitivityTests, validationContext);

            result.MemberNames.Should().BeEquivalentTo(new List<string>{"ValidatedProperty"});
            result.ErrorMessage.Should().NotBeEmpty();
        }

        private SensitivityTestAttribute CreateSut()
        {
            return new SensitivityTestAttribute();
        }
    }
}