﻿using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Utils;

namespace NRZMyk.Server.ModelExtensions
{
    public static class SentinelEntryExtensions
    {
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
    }
}