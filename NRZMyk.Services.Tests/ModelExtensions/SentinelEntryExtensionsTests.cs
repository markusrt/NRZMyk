using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.ModelExtensions;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.ModelExtensions;

public class SentinelEntryExtensionsTests
{
    [Test]
    public void WhenHospitalDepartmentIsOther_TakesOtherValue()
    {
        var sentinelEntry = new SentinelEntry();
        sentinelEntry.HospitalDepartment = HospitalDepartment.Other;
        sentinelEntry.OtherHospitalDepartment = "Foo";

        sentinelEntry.HospitalDepartmentOrOther().Should().Be("Foo");
    }

    [Test]
    public void WhenHospitalDepartmentIsSet_TakesEnumDescription()
    {
        var sentinelEntry = new SentinelEntry();
        sentinelEntry.HospitalDepartment = HospitalDepartment.Anaesthetics;

        sentinelEntry.HospitalDepartmentOrOther().Should().Be("anästhesiologisch");
    }

    [Test]
    public void WhenHospitalDepartmentIsInternalNormalUnit_AddsAdditionalType()
    {
        var sentinelEntry = new SentinelEntry();
        sentinelEntry.HospitalDepartment = HospitalDepartment.Internal;
        sentinelEntry.InternalHospitalDepartmentType = InternalHospitalDepartmentType.Cardiological;

        sentinelEntry.HospitalDepartmentOrOther().Should().Be("internistisch, kardiologisch");
    }
}