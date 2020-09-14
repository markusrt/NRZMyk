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
                breakPoint.OverwriteId(id++);
                _repository.Add(breakPoint);
            }
        }

        private string ReadBreakpointJson()
        {
            var name = "NRZMyk.Mocks.Data.ClinicalBreakpoints.json";
            using var stream = this.GetType().Assembly.
                GetManifestResourceStream(name);
            using var sr = new StreamReader(stream);                           
             
            return sr.ReadToEnd();
        }

        public Task<List<ClinicalBreakpoint>> List()
        {
            return Task.FromResult(_repository);
        }

        private sealed class MockClinicalBreakPoint : ClinicalBreakpoint
        {
            public void OverwriteId(int id)
            {
                Id = id;
            }
        }
    }
}