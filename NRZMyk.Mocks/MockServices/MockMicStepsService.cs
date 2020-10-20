using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;

namespace NRZMyk.Mocks.MockServices
{
    public class MockMicStepsService : MicStepsService
    {
        public List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent)
        {
            return new List<MicStep>()
            {
                new MicStep(){Title = "≤4", Value = 1},
                new MicStep(){Title = "8", Value = 8},
                new MicStep(){Title = "≥16", Value = 16}
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

        public bool IsMultiAgentSystem(SpeciesTestingMethod testingMethod)
        {
            return true;
        }
    }
}