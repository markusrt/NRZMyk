using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Services
{
    public interface MicStepsService
    {
        List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent);
    }

    public class MicStepsServiceImpl : MicStepsService
    {
        private Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>> _micSteps;

        public MicStepsServiceImpl(IOptions<BreakpointSettings> config)
        {
            _micSteps = config.Value?.Breakpoint?.MicSteps??new Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>>();
        }

        public List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent)
        {
            List<MicStep> agentSteps = null;
            _micSteps.TryGetValue(testingMethod, out var testingMethodSteps);
            testingMethodSteps?.TryGetValue(agent, out agentSteps);
            return agentSteps ?? new List<MicStep>();
        }
    }
}