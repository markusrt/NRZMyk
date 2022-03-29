using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NRZMyk.Services.Validation;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Validation
{
    public class RequiredForNormalInternalUnitTests
    {
        [Test]
        public void WhenValueIsNull_IsTreatedAsValid()
        {
            var sut = CreateSut();
            SentinelEntryRequest emptyRequest = null;

            sut.IsValid(emptyRequest).Should().BeTrue();
        }

        [Test]
        public void WhenValueIsOtherType_IsTreatedAsValid()
        {
            var sut = CreateSut();
            var wrongType = new List<string> { "Foo" };

            sut.IsValid(wrongType).Should().BeTrue();
        }

        [Test]
        public void WhenValueIsMissingForNonNormalInternalUnit_IsTreatedAsValid()
        {
            var sut = CreateSut();
            var noNormalUnit = new SentinelEntryRequest
            {
                HospitalDepartmentType = HospitalDepartmentType.IntensiveCareUnit,
                HospitalDepartment = HospitalDepartment.Internal,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment
            };

            var validationContext = new ValidationContext(noNormalUnit) { MemberName = "ValidatedProperty" };
            var result = sut.GetValidationResult(noNormalUnit, validationContext);

            result.Should().BeNull();
        }

        [Test]
        public void WhenValueIsSetForNonNormalInternalUnit_IsTreatedAsValid()
        {
            var sut = CreateSut();
            var noNormalUnit = new SentinelEntryRequest
            {
                HospitalDepartmentType = HospitalDepartmentType.IntensiveCareUnit,
                HospitalDepartment = HospitalDepartment.Internal,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.Cardiological
            };

            var validationContext = new ValidationContext(noNormalUnit) { MemberName = "ValidatedProperty" };
            var result = sut.GetValidationResult(noNormalUnit, validationContext);

            result.Should().NotBeNull();
            result?.MemberNames.Should().BeEquivalentTo(new List<string> { "ValidatedProperty" });
            result?.ErrorMessage.Should().NotBeEmpty();
        }

        [Test]
        public void WhenValueIsMissingForNormalUnit_IsTreatedAsInvalid()
        {
            var sut = CreateSut();
            var noNormalUnit = new SentinelEntryRequest
            {
                HospitalDepartmentType = HospitalDepartmentType.NormalUnit,
                HospitalDepartment = HospitalDepartment.Internal,
                InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment
            };
            var validationContext = new ValidationContext(noNormalUnit) { MemberName = "ValidatedProperty" };
            var result = sut.GetValidationResult(noNormalUnit, validationContext);
            
            result.Should().NotBeNull();
            result?.MemberNames.Should().BeEquivalentTo(new List<string> { "ValidatedProperty" });
            result?.ErrorMessage.Should().NotBeEmpty();
        }

        [TestCaseSource(nameof(InternalHospitalDepartmentTypes))]
        public void WhenValueIsSetForNormalUnit_IsTreatedAsValid(InternalHospitalDepartmentType validType)
        {
            var sut = CreateSut();
            var noNormalUnit = new SentinelEntryRequest
            {
                HospitalDepartmentType = HospitalDepartmentType.NormalUnit,
                HospitalDepartment = HospitalDepartment.Internal,
                InternalHospitalDepartmentType = validType
            };
            var validationContext = new ValidationContext(noNormalUnit) { MemberName = "ValidatedProperty" };
            var result = sut.GetValidationResult(noNormalUnit, validationContext);
            
            result.Should().BeNull();
        }

        public static IEnumerable<InternalHospitalDepartmentType> InternalHospitalDepartmentTypes
            => Enum.GetValues<InternalHospitalDepartmentType>().Where(e => e != InternalHospitalDepartmentType.NoInternalDepartment);

        private RequiredForNormalInternalUnit CreateSut()
        {
            return new RequiredForNormalInternalUnit();
        }
    }
}