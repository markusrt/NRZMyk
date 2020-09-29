﻿using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NRZMyk.Services.Utils;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Utils
{
    [TestFixture]
    public class EnumUtilsTests
    {
        [Test]
        [TestCase("Flag2", ClinicalInformation.Flag2)]
        [TestCase("Flag1,,Flag2", ClinicalInformation.Flag2 | ClinicalInformation.Flag1)]
        [TestCase("", ClinicalInformation.None)]
        [TestCase(null, ClinicalInformation.None)]
        public void ParseCommaSeparatedListOfNamesAsFlagsEnum_ParsesCorrectly(string commaSeperatedList,
            ClinicalInformation expectedResult)
        {
            var result = EnumUtils.ParseCommaSeperatedListOfNamesAsFlagsEnum<ClinicalInformation>(commaSeperatedList);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void ParseCommaSeparatedListOfNames_ValidList_ParsesCorrectly()
        {
            var commaSeperatedList = "Two, One";
            var expectedResult = new List<UtilsTest> {UtilsTest.Two, UtilsTest.One};

            var result = EnumUtils.ParseCommaSeperatedListOfNames<UtilsTest>(commaSeperatedList);

            result.Should().ContainInOrder(expectedResult);
        }

        [Test]
        public void ParseCommaSeparatedListOfNames_EmptyList_EmptyResult()
        {
            var commaSeperatedList = ", ";

            var result = EnumUtils.ParseCommaSeperatedListOfNames<UtilsTest>(commaSeperatedList);

            result.Should().BeEmpty();
        }

        [Test]
        public void ParseCommaSeparatedListOfNames_InvalidEntry_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => EnumUtils.ParseCommaSeperatedListOfNames<UtilsTest>("Zero, Foo, One"));
        }



        [Test]
        public void AllEnumValues_NotAnEnum_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => EnumUtils.AllEnumValues<int>());
        }

        [Test]
        public void AllEnumValues_ValidEnum_ReturnsAllValues()
        {
            var values = EnumUtils.AllEnumValues<UtilsTest>();

            values.Should().BeEquivalentTo(new[] {UtilsTest.Zero, UtilsTest.One, UtilsTest.Two});
        }

        [Test]
        public void FirstAttribute_AttributeDoesNotExist_ReturnsNull()
        {
            var attribute = UtilsTest.Zero.FirstAttribute<MockAttribute>();

            attribute.Should().BeNull();
        }

        [Test]
        public void FirstAttribute_AttributeExists_DoesNotReturnNull()
        {
            var attribute = UtilsTest.One.FirstAttribute<MockAttribute>();

            attribute.Should().NotBeNull();
            attribute.GetType().Should().Be(typeof (MockAttribute));
        }

        [Test]
        public void IsDefinedEnumValue_WhenPassedACombinationWhichAlsoExistsAsExplicitEnumEntry_ReturnsTrue()
        {
            var definedFlagValueOfCombination = (FileAccess.Read | FileAccess.Write).IsDefinedEnumValue();
            var definedFlagValueOfEquivalentSingleEntry = FileAccess.ReadWrite.IsDefinedEnumValue();

            definedFlagValueOfCombination.Should().BeTrue();
            definedFlagValueOfEquivalentSingleEntry.Should().BeTrue();
        }

        private readonly UtilsTest? ZeroAsNullable = UtilsTest.Zero;

        [TestCase(0)]
        [TestCase("Zero")]
        [TestCase(UtilsTest.Zero)]
        public void GetEnumDescription_Attribute_ReturnsDescription(object value)
        {
            EnumUtils.GetEnumDescription<UtilsTest>(value).Should().Be("Null");
        }

        [TestCase(UtilsTest.Zero, "Null")]
        [TestCase(1, "Eins")]
        public void GetEnumDescription_NullableType_ReturnsDescription(object value, string expected)
        {
            EnumUtils.GetEnumDescription<UtilsTest?>(value).Should().Be(expected);
        }

        [Test]
        public void GetEnumDescription_GenericVersion_ReturnsDescription()
        {
            EnumUtils.GetEnumDescription(UtilsTest.One).Should().Be("Eins");
        }

        [Test]
        public void GetEnumDescription_NoAttribute_ReturnsEnumName()
        {
            EnumUtils.GetEnumDescription<UtilsTest>("Two").Should().Be("Two");
        }

        [Test]
        public void GetEnumDescription_NoEnum_ReturnsValueAsString()
        {
            EnumUtils.GetEnumDescription("Foobar").Should().Be("Foobar");
        }

        [Test]
        [TestCase(3, "3")]
        [TestCase("Three", "Three")]
        public void GetEnumDescription_NoEnumEntry_ReturnsInput(object value, string expected)
        {
            EnumUtils.GetEnumDescription<UtilsTest>(value).Should().Be(expected);
        }
    }

    public enum UtilsTest
    {
        [System.ComponentModel.Description("Null")] Zero = 0,
        [Mock] [System.ComponentModel.Description("Eins")] One = 1,
        Two = 2
    }

    [Flags]
    public enum ClinicalInformation
    {
        None = 0,
        Flag1 = 2,
        Flag2 = 4,
        Flag3 = 8,
    }

    public class MockAttribute : Attribute
    {
    }
}