using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Interfaces
{
    public interface IMicStepsService
    {
        List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent);
        IEnumerable<SpeciesTestingMethod> TestingMethods();
        IEnumerable<AntifungalAgent> AntifungalAgents(SpeciesTestingMethod testingMethod);
        IEnumerable<BrothMicrodilutionStandard> Standards(SpeciesTestingMethod testingMethod);
        bool IsMultiAgentSystem(SpeciesTestingMethod testingMethod);
    }
}