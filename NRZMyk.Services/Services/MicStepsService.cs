using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Services
{
    public class MicStepsService : IMicStepsService
    {
        private readonly ILogger<MicStepsService> _logger;
        private readonly Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>> _micSteps;
        private readonly List<SpeciesTestingMethod> _multiAgentSystems;
        private Dictionary<SpeciesTestingMethod, List<BrothMicrodilutionStandard>> _standards;

        public MicStepsService(IOptions<BreakpointSettings> config, ILogger<MicStepsService> logger)
        {
            _logger = logger;
            _micSteps = config.Value?.Breakpoint?.MicSteps??new Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>>();
            _multiAgentSystems = config.Value?.Breakpoint?.MultiAgentSystems ?? new List<SpeciesTestingMethod>();
            _standards = config.Value?.Breakpoint?.Standards ??
                         new Dictionary<SpeciesTestingMethod, List<BrothMicrodilutionStandard>>();
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

            agentSteps.First().LowerBoundary = true;
            agentSteps.Last().UpperBoundary = true;
            return agentSteps;
        }

        public IEnumerable<SpeciesTestingMethod> TestingMethods()
        {
            return _micSteps.Keys;
        }

        public IEnumerable<AntifungalAgent> AntifungalAgents(SpeciesTestingMethod speciesTestingMethod)
        {
            var antifungalAgents = _micSteps[speciesTestingMethod].Keys;
            return antifungalAgents.ToImmutableSortedSet(new AntifungalAgentComparer());
        }

        public IEnumerable<BrothMicrodilutionStandard> Standards(SpeciesTestingMethod testingMethod)
        {
            _standards.TryGetValue(testingMethod, out var standards);
            return standards ?? EnumUtils.AllEnumValues<BrothMicrodilutionStandard>().Where(t => t != BrothMicrodilutionStandard.None);
        }

        public bool IsMultiAgentSystem(SpeciesTestingMethod testingMethod)
        {
            return _multiAgentSystems.Contains(testingMethod);
        }
    }
}