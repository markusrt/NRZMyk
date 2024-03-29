﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Models;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.Services
{
    public class MicStepsService : IMicStepsService
    {
        private const string Config = "NRZMyk.Services.Configuration.BreakpointSettings.json";

        private readonly ILogger<MicStepsService> _logger;
        private readonly Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>> _micSteps;
        private readonly List<SpeciesTestingMethod> _multiAgentSystems;
        private readonly Dictionary<SpeciesTestingMethod, List<BrothMicrodilutionStandard>> _standards;
        private readonly List<float> _referenceMethodMicValues;

        public MicStepsService(ILogger<MicStepsService> logger) : this(LoadSettingsFromResource(), logger)
        {
        }

        public MicStepsService(BreakpointSettings settings, ILogger<MicStepsService> logger)
        {
            _logger = logger;
            _micSteps = settings.Breakpoint?.MicSteps??new Dictionary<SpeciesTestingMethod, Dictionary<AntifungalAgent, List<MicStep>>>();
            _multiAgentSystems = settings.Breakpoint?.MultiAgentSystems ?? new List<SpeciesTestingMethod>();
            _referenceMethodMicValues = settings.Breakpoint?.ReferenceMethodMicValues ?? new List<float>();
            _standards = settings.Breakpoint?.Standards ??
                         new Dictionary<SpeciesTestingMethod, List<BrothMicrodilutionStandard>>();
        }

        public List<MicStep> StepsByTestingMethodAndAgent(SpeciesTestingMethod testingMethod, AntifungalAgent agent)
        {
            List<MicStep> agentSteps = null;
            _micSteps.TryGetValue(testingMethod, out var testingMethodSteps);
            testingMethodSteps?.TryGetValue(agent, out agentSteps);

            if (agentSteps == null)
            {
                _logger.LogInformation("No MIC steps for {testingMethod}/{agent} found", testingMethod, agent);
                return new List<MicStep>();
            }
            _logger.LogInformation("Found {stepCount} MIC steps for {testingMethod}/{agent} found",
                agentSteps.Count, testingMethod, agent);

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

        private static BreakpointSettings LoadSettingsFromResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(Config);
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<BreakpointSettings>(json);
        }

        public float? FloorToClosestReferenceValue(float? micValue)
        {
            var flooredValue = micValue;
            foreach (var referenceMethodMicValue in _referenceMethodMicValues.OrderBy(v => v))
            {
                if (referenceMethodMicValue <= micValue)
                {
                    flooredValue = referenceMethodMicValue;
                }
            }
            return flooredValue;
        }
    }
}