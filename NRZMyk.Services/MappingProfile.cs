﻿using AutoMapper;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Models;
using NRZMyk.Services.Services;

namespace NRZMyk.Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSentinelEntryRequest, SentinelEntry>();
            CreateMap<ClinicalBreakpoint, ClinicalBreakpointReference>();
        }
    }
}