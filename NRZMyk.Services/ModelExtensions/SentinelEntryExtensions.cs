using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;
using NRZMyk.Services.Utils;

namespace NRZMyk.Services.ModelExtensions
{
    public static class SentinelEntryExtensions
    {
        public static string MaterialOrOther(this SentinelEntryRequest sentinelEntry)
        {
            return sentinelEntry.Material == Material.Other
                ? sentinelEntry.OtherMaterial
                : EnumUtils.GetEnumDescription(sentinelEntry.Material);
        }

        public static string MaterialOrOther(this SentinelEntry sentinelEntry)
        {
            return sentinelEntry.Material == Material.Other
                ? sentinelEntry.OtherMaterial
                : EnumUtils.GetEnumDescription(sentinelEntry.Material);
        }

        public static string SpeciesOrOther(this SentinelEntry sentinelEntry)
        {
            return sentinelEntry.IdentifiedSpecies == Species.Other
                ? sentinelEntry.OtherIdentifiedSpecies
                : EnumUtils.GetEnumDescription(sentinelEntry.IdentifiedSpecies);
        }

        public static string HospitalDepartementOrOther(this SentinelEntry sentinelEntry)
        {
            return sentinelEntry.HospitalDepartment == HospitalDepartment.Other
                ? sentinelEntry.OtherHospitalDepartment
                : EnumUtils.GetEnumDescription(sentinelEntry.HospitalDepartment);
        }

        public static string SpeciesIdentificationMethodWithPcrDetails(this SentinelEntry sentinelEntry)
        {
            return sentinelEntry.SpeciesIdentificationMethod == SpeciesIdentificationMethod.Pcr
                ? $"{EnumUtils.GetEnumDescription(SpeciesIdentificationMethod.Pcr)}: {sentinelEntry.PcrDetails}"
                : EnumUtils.GetEnumDescription(sentinelEntry.IdentifiedSpecies);
        }
    }
}