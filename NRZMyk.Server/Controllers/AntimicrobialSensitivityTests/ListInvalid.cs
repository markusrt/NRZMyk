using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Extensions;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Specifications;
using Swashbuckle.AspNetCore.Annotations;

namespace NRZMyk.Server.Controllers.AntimicrobialSensitivityTests
{
    [Authorize]
    public class ListInvalid : BaseAsyncEndpoint<List<AntimicrobialSensitivityTest>>
    {
        private readonly IAsyncRepository<AntimicrobialSensitivityTest> _repository;

        private readonly IMicStepsService _micStepsService;

        public ListInvalid(IAsyncRepository<AntimicrobialSensitivityTest> repository, IMicStepsService micStepsService)
        {
            _repository = repository;
            _micStepsService = micStepsService;
        }

        [HttpGet("api/antimicrobial-sensitivity-tests/invalid")]
        [SwaggerOperation(
            Summary = "List invalid  antimicrobial sensitivity tests",
            OperationId = "antimicrobial-sensitivity-tests.listInvalid",
            Tags = new[] { "SharedEndpoints" })
        ]
        public override async Task<ActionResult<List<AntimicrobialSensitivityTest>>> HandleAsync()
        {
            var tests = await _repository.ListAsync(new AntimicrobialSensitivityTestIncludingBreakpointSpecification()).ConfigureAwait(false);
            var incorrectTests = new List<AntimicrobialSensitivityTest>();

            foreach (var test in tests)
            {
                if (test.ClinicalBreakpoint ==  null || test.Resistance == Resistance.NotEvaluable)
                {
                    continue;
                }

                var currentMic = test.MinimumInhibitoryConcentration;
                var flooredMic = _micStepsService.FloorToClosestReferenceValue(currentMic);
                if (currentMic.Equals(flooredMic))
                {
                    continue;
                }

                if (flooredMic.IsResistantAccordingToClsiDefinition(test.ClinicalBreakpoint))
                {
                    if(test.Resistance != Resistance.Resistant) incorrectTests.Add(test);
                }
                else if (flooredMic.IsResistantAccordingToEucastDefinition(test.ClinicalBreakpoint))
                {
                    if(test.Resistance != Resistance.Resistant) incorrectTests.Add(test);
                }
                else if (flooredMic.IsSusceptibleAccordingToBothDefinitions(test.ClinicalBreakpoint))
                {
                    if(test.Resistance != Resistance.Susceptible) incorrectTests.Add(test);
                }
            }
            return Ok(incorrectTests);
        }
    }
}
