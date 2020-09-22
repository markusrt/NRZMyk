using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;

namespace NRZMyk.Services.Services
{
    public interface MicStepsService
    {
        List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent);
        IEnumerable<SpeciesTestingMethod> TestingMethods();
        IEnumerable<AntifungalAgent> AntifungalAgents(SpeciesTestingMethod testingMethod);
    }

    public class MicStepsServiceImpl : MicStepsService
    {
        private readonly ILogger<MicStepsServiceImpl> _logger;
        private Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>> _micSteps;

        public MicStepsServiceImpl(IOptions<BreakpointSettings> config, ILogger<MicStepsServiceImpl> logger)
        {
            _logger = logger;
            _micSteps = config.Value?.Breakpoint?.MicSteps??new Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>>();
        }

        public List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent)
        {
            List<MicStep> agentSteps = null;
            _micSteps.TryGetValue(testingMethod, out var testingMethodSteps);
            testingMethodSteps?.TryGetValue(agent, out agentSteps);

            if (agentSteps == null)
            {
                _logger.LogInformation($"No MIC steps for {testingMethod}/{agent} found");
                return new List<MicStep>();
            }
            _logger.LogInformation($"Found {agentSteps.Count} MIC steps for {testingMethod}/{agent} found");

            return agentSteps;
        }

        public IEnumerable<SpeciesTestingMethod> TestingMethods()
        {
            return _micSteps.Keys;
        }

        public IEnumerable<AntifungalAgent> AntifungalAgents(SpeciesTestingMethod speciesTestingMethod)
        {
            return _micSteps[speciesTestingMethod].Keys;
        }
    }
}