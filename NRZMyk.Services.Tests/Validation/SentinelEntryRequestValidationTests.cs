using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Services;
using NRZMyk.Services.Validation;
using NSubstitute;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Validation
{
    public class SentinelEntryRequestValidationTests
    {
        [Test]
        public void WhenIncorrectParentTypeForSensitivityTestNotEmptyWithoutComment_IsTreatedAsValid()
        {
            var target = new InvalidType();
            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();
            
            var isValid = Validator.TryValidateObject(target, context, results, true);

            Assert.That(isValid, Is.True);
        }

        //[Test]
        //public void WhenMissingTestsAndNoComment_IsTreatedAsValid()
        //{
        //    var target = new SentinelEntryRequest {AntimicrobialSensitivityTests = null};
        //    var context = new ValidationContext(target);
        //    var results = new List<ValidationResult>();
            
        //    Validator.TryValidateObject(target, context, results, true);

        //    var result = results.SingleOrDefault(r =>
        //        r.MemberNames.Contains(nameof(SentinelEntryRequest.AntimicrobialSensitivityTests)));
        //    result.Should().BeNull();
        //}

        //[TestCase(null)]
        //[TestCase("")]
        //[TestCase("   ")]
        //public void WhenEmptyTestsAndNoComment_IsTreatedAsInvalid(string remark)
        //{
        //    var target = new SentinelEntryRequest()
        //    {
        //        AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTestRequest>(),
        //        Remark = remark
        //    };
        //    var context = new ValidationContext(target);
        //    var results = new List<ValidationResult>();
            
        //    Validator.TryValidateObject(target, context, results, true);

        //    var result = results.SingleOrDefault(r =>
        //        r.MemberNames.Contains(nameof(SentinelEntryRequest.AntimicrobialSensitivityTests)));
        //    result.Should().NotBe(null, $"{nameof(SentinelEntryRequest.AntimicrobialSensitivityTests)} should be invalid");
        //}

        //[Test]
        //public void WhenEmptyTestsWithComment_IsTreatedAsValid()
        //{
        //    var target = new SentinelEntryRequest()
        //    {
        //        AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTestRequest>(),
        //        Remark = "We were to lazy for sensitivity tests :/"
        //    };
        //    var context = new ValidationContext(target);
        //    var results = new List<ValidationResult>();
            
        //    Validator.TryValidateObject(target, context, results, true);

        //    var result = results.SingleOrDefault(r =>
        //        r.MemberNames.Contains(nameof(SentinelEntryRequest.AntimicrobialSensitivityTests)));
        //    result.Should().BeNull();
        //}

        //[Test]
        //public void WhenNonEmptyTestsWithoutComment_IsTreatedAsValid()
        //{
        //    var target = new SentinelEntryRequest()
        //    {
        //        AntimicrobialSensitivityTests = new List<AntimicrobialSensitivityTestRequest>
        //        {
        //            new(){ MinimumInhibitoryConcentration = 1.0f }
        //        },
        //        Remark = ""
        //    };
        //    var context = new ValidationContext(target);
        //    var results = new List<ValidationResult>();
            
        //    Validator.TryValidateObject(target, context, results, true);

        //    var result = results.SingleOrDefault(r =>
        //        r.MemberNames.Contains(nameof(SentinelEntryRequest.AntimicrobialSensitivityTests)));
        //    result.Should().BeNull();
        //}

        [Test]
        public void WhenSamplingDateIsMissing_IsTreatedAsInvalid()
        {
            var target = new SentinelEntryRequest()
            {
                SamplingDate = null
            };
            var context = new ValidationContext(target);
            var results = new List<ValidationResult>();
            
            Validator.TryValidateObject(target, context, results, true);

            var result = results.SingleOrDefault(r =>
                r.MemberNames.Contains(nameof(SentinelEntryRequest.SamplingDate)));
            result.Should().NotBe(null, $"{nameof(SentinelEntryRequest.SamplingDate)} should be required");
        }

        private SensitivityTestNotEmptyWithoutCommentAttribute CreateSut()
        {
            return new SensitivityTestNotEmptyWithoutCommentAttribute();
        }

        private class InvalidType
        {
            public string X { get; set; }

            [SensitivityTestNotEmptyWithoutComment]
            public List<AntimicrobialSensitivityTestRequest> AntimicrobialSensitivityTests { get; set; }
        }
    }
}