using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Playground.MockServices
{
    public class MockClinicalBreakpointServiceImpl : ClinicalBreakpointService
    {
        private readonly List<ClinicalBreakpointReference> _repository = new List<ClinicalBreakpointReference>();

        public MockClinicalBreakpointServiceImpl()
        {
            _repository.Add(new ClinicalBreakpointReference
            {
                Id = 1,
                Title = "Breakpoint reference 1"
            });
            _repository.Add(new ClinicalBreakpointReference
            {
                Id = 2,
                Title = "Breakpoint reference 2"
            });
        }

        public Task<List<ClinicalBreakpointReference>> List()
        {
            return Task.FromResult(_repository);
        }
    }
}