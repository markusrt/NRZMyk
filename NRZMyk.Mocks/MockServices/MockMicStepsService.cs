using System.Collections.Generic;
using System.Linq;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;
using NRZMyk.Services.Utils;

namespace NRZMyk.Mocks.MockServices
{
    public class MockMicStepsService : IMicStepsService
    {
        public List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent)
        {
            return new List<MicStep>()
            {
                new MicStep {Title = "≤4", Value = 4, LowerBoundary = true},
                new MicStep {Title = "8", Value = 8},
                new MicStep {Title = "≥16", Value = 16, UpperBoundary = true}
            };
        }

        public IEnumerable<SpeciesTestingMethod> TestingMethods()
        {
            return new List<SpeciesTestingMethod> {SpeciesTestingMethod.Vitek};
        }

        public IEnumerable<AntifungalAgent> AntifungalAgents(SpeciesTestingMethod testingMethod)
        {
            return new List<AntifungalAgent> {AntifungalAgent.AmphotericinB};
        }

        public IEnumerable<BrothMicrodilutionStandard> Standards(SpeciesTestingMethod testingMethod)
        {
            return EnumUtils.AllEnumValues<BrothMicrodilutionStandard>().Where(t => t != BrothMicrodilutionStandard.None);
        }

        public bool IsMultiAgentSystem(SpeciesTestingMethod testingMethod)
        {
            return true;
        }

        public float? FloorToClosestReferenceValue(float? micValue)
        {
            return micValue.Equals(0.064f) ? 0.06f : micValue;
        }
    }
}