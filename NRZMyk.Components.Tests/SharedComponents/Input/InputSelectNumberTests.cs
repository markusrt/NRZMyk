using Bunit;
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

        [TestCase("-1", -1)]
        [TestCase("0", 0)]
        [TestCase("1000000", 1000000)]
        public void WhenNumberSelected_NullableIntegerValueIsParsed(string valueAsString, int? expectedValue)
        {
            var sut = CreateSut<int?>("TeamSize");

            sut.InvokeTryParseValueFromString(valueAsString, out var actualValue, out var errorMessage)
                .Should().BeTrue();

            actualValue.Should().Be(expectedValue);
            errorMessage.Should().BeNull();
        }

        [TestCase("", null)]
        [TestCase(null, null)]
        public void WhenNumberSelected_NullableIntegerValueIsParsedAsNull(string valueAsString, int? expectedValue)
        {
            var sut = CreateSut<int?>("TeamSize");

            sut.InvokeTryParseValueFromString(valueAsString, out var actualValue, out var errorMessage)
                .Should().BeFalse();

            actualValue.Should().Be(expectedValue);
            errorMessage.Should().Be("The field 'TeamSize' does not contain a valid number.");
        }

        [TestCase("-100.000")]
        [TestCase("123.111")]
        [TestCase("0,0001")]
        [TestCase("0.0001")]
        [TestCase("100,000")]
        [TestCase("0")]
        public void WhenNumberSelected_FloatValueIsParsed(string valueAsString)
        {
            var sut = CreateSut<float>("Measurement");

            sut.InvokeTryParseValueFromString(valueAsString, out var actualValue, out var errorMessage)
                .Should().BeTrue();

            actualValue.Should().Be(float.Parse(valueAsString));
            errorMessage.Should().BeNull();
        }

        [TestCase("-100.000")]
        [TestCase("123.111")]
        [TestCase("0,0001")]
        [TestCase("0.0001")]
        [TestCase("100,000")]
        [TestCase("0")]
        public void WhenNumberSelected_NullableFloatValueIsParsed(string valueAsString)
        {
            var sut = CreateSut<float?>("Measurement");

            sut.InvokeTryParseValueFromString(valueAsString, out var actualValue, out var errorMessage)
                .Should().BeTrue();

            actualValue.Should().Be(float.Parse(valueAsString));
            errorMessage.Should().BeNull();
        }

        [TestCase("")]
        [TestCase(null)]
        public void WhenNumberSelectedIsEmpty_NullableFloatValueIsParsed(string valueAsString)
        {
            var sut = CreateSut<float?>("Measurement");

            sut.InvokeTryParseValueFromString(valueAsString, out var actualValue, out var errorMessage)
                .Should().BeFalse();

            actualValue.Should().BeNull();
            errorMessage.Should().Be("The field 'Measurement' does not contain a valid number.");
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

        private static TestableInputSelectNumber<T> CreateSut<T>(string fieldName)
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