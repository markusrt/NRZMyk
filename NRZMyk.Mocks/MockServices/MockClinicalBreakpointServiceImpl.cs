using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Mocks.MockServices
{
    public class MockClinicalBreakpointServiceImpl : ClinicalBreakpointService
    {
        private readonly List<ClinicalBreakpoint> _repository = new List<ClinicalBreakpoint>();

        public MockClinicalBreakpointServiceImpl()
        {
            var id = 1;
            var clinicalBreakpointJson = ReadBreakpointJson();
            foreach (var breakPoint in JsonConvert.DeserializeObject<IEnumerable<MockClinicalBreakPoint>>(clinicalBreakpointJson))
            {
                AddBreakpoint(id++, breakPoint);
            }
        }

        public void AddBreakpoint(int id, MockClinicalBreakPoint breakPoint)
        {
            breakPoint.OverwriteId(id++);
            _repository.Add(breakPoint);
        }

        private string ReadBreakpointJson()
        {
            using var stream = GetType().Assembly.
                GetManifestResourceStream("NRZMyk.Mocks.Data.ClinicalBreakpoints.json");
            using var streamReader = new StreamReader(stream);                           
            return streamReader.ReadToEnd();
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