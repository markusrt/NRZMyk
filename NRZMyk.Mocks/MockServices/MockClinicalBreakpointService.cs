using System.Collections.Generic;
using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Mocks.MockServices
{
    public class MockClinicalBreakpointService : IClinicalBreakpointService
    {
        private readonly List<ClinicalBreakpoint> _repository = new List<ClinicalBreakpoint>();

        public MockClinicalBreakpointService() : this(new ClinicalBreakpointProvider())
        {
        }

        public MockClinicalBreakpointService(IClinicalBreakpointProvider breakpointProvider)
        {
            var id = 1;
            foreach (var breakPoint in breakpointProvider.GetBreakpoints())
            {
                var mockBreakpoint = new MockClinicalBreakPoint
                {
                    AntifungalAgent = breakPoint.AntifungalAgent,
                    AntifungalAgentDetails = breakPoint.AntifungalAgentDetails,
                    Species = breakPoint.Species,
                    Standard = breakPoint.Standard,
                    Version = breakPoint.Version,
                    ValidFrom = breakPoint.ValidFrom,
                    MicBreakpointSusceptible = breakPoint.MicBreakpointSusceptible,
                    MicBreakpointResistent = breakPoint.MicBreakpointResistent,
                    TechnicalUncertainty = breakPoint.TechnicalUncertainty
                };
                AddBreakpoint(id++, mockBreakpoint);
            }
        }

        public void AddBreakpoint(int id, MockClinicalBreakPoint breakPoint)
        {
            breakPoint.OverwriteId(id++);
            _repository.Add(breakPoint);
        }

        public Task<List<ClinicalBreakpoint>> List()
        {
            return Task.FromResult(_repository);
        }
        
        public sealed class MockClinicalBreakPoint : ClinicalBreakpoint
        {
            public void OverwriteId(int id)
            {
                Id = id;
            }
        }
    }
}