using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Playground.MockServices
{
    public class MockClinicalBreakpointServiceImpl : ClinicalBreakpointService
    {
        private readonly List<ClinicalBreakpoint> _repository = new List<ClinicalBreakpoint>();

        public MockClinicalBreakpointServiceImpl()
        {
            _repository.Add(new MockClinicalBreakPoint(1)
            {
                AntifungalAgent = AntifungalAgent.Fluconazole,
                AntifungalAgentDetails = "Fluconazole - 0.25",
                Version = "1.0",
                MicBreakpointResistent = 0.25f,
                MicBreakpointSusceptible = 0.25f,
            });
            _repository.Add(new MockClinicalBreakPoint(2)
            {
                AntifungalAgent = AntifungalAgent.AmphotericinB,
                AntifungalAgentDetails = "AmphotericinB - 0.5",
                Version = "2.0",
                MicBreakpointResistent = 0.5f,
                MicBreakpointSusceptible = 0.5f,
            });
        }

        public Task<List<ClinicalBreakpoint>> List()
        {
            return Task.FromResult(_repository);
        }

        private sealed class MockClinicalBreakPoint : ClinicalBreakpoint
        {
            public MockClinicalBreakPoint(int id)
            {
                Id = id;
            }
        }
    }
}