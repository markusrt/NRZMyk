﻿using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Forms;
using NRZMyk.Components.SharedComponents.Input;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NRZMyk.ComponentsTests.SharedComponents.Input
{
    public class InputSelectNumberTests 
    {
        [TestCase("-1", -1)]
        [TestCase("0", 0)]
        [TestCase("1000000", 1000000)]
        public void WhenNumberSelected_IntegerValueIsParsed(string valueAsString, int expectedValue)
        {
            var sut = CreateSut<int>("TeamSize");

            sut.InvokeTryParseValueFromString(valueAsString, out var actualValue, out var errorMessage)
                .Should().BeTrue();

            actualValue.Should().Be(expectedValue);
            errorMessage.Should().BeNull();
        }

        [TestCase("-100.000", -100000.0f)]
        [TestCase("123.111.230", 123111230f)]
        [TestCase("0,0001", 0.0001f)]
        [TestCase("1000000", 1000000.0f)]
        [TestCase("0", 0.0f)]
        public void WhenNumberSelected_FloatValueIsParsed(string valueAsString, float expectedValue)
        {
            var sut = CreateSut<float>("Measurement");

            sut.InvokeTryParseValueFromString(valueAsString, out var actualValue, out var errorMessage)
                .Should().BeTrue();

            actualValue.Should().Be(expectedValue);
            errorMessage.Should().BeNull();
        }

        [Test]
        public void WhenEmptyString_ErrorIsReturned()
        {
            var sut = CreateSut<int>("TeamSize");

            sut.InvokeTryParseValueFromString(string.Empty, out var actualValue, out var errorMessage)
                .Should().BeFalse();

            actualValue.Should().Be(default);
            errorMessage.Should().Be("The field 'TeamSize' does not contain a valid number.");
        }

        private TestableInputSelectNumber<T> CreateSut<T>(string fieldName)
        {
            return new TestableInputSelectNumber<T>(fieldName);
        }

        private class TestableInputSelectNumber<T> : InputSelectNumber<T>
        {
            public TestableInputSelectNumber(string fieldName)
            {
               FieldIdentifier = new FieldIdentifier(new object(), fieldName);
            }

            public bool InvokeTryParseValueFromString(string value, out T result, out string validationErrorMessage)
            {
                return TryParseValueFromString(value, out result, out validationErrorMessage);
            }
        }
    }
}