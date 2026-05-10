using System;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using Tynamix.ObjectFiller;

namespace Api.Integration.Tests.SentinelEntries
{
    internal static class SentinelEntryTestHelper
    {
        internal static SentinelEntryRequest CreateValidSentinelEntryRequest()
        {
            var filler = new Filler<SentinelEntryRequest>();
            var request = filler.Create();
            request.Material = Material.CentralBloodCultureOther;
            request.HospitalDepartment = HospitalDepartment.GeneralSurgery;
            request.InternalHospitalDepartmentType = InternalHospitalDepartmentType.NoInternalDepartment;
            request.IdentifiedSpecies = Species.CandidaDubliniensis;
            request.SpeciesIdentificationMethod = SpeciesIdentificationMethod.BBL;
            request.SamplingDate = DateTime.Now.AddDays(-3);
            request.PredecessorLaboratoryNumber = string.Empty;
            request.HasPredecessor = YesNo.No;
            return request;
        }
    }
}