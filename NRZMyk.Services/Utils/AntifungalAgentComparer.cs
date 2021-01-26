using System.Collections.Generic;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Utils
{
    public class AntifungalAgentComparer : IComparer<AntifungalAgent> {
        private static readonly List<AntifungalAgent> AgentsOrderedByGroup = new List<AntifungalAgent>
        {
            AntifungalAgent.AmphotericinB,
            AntifungalAgent.Anidulafungin,
            AntifungalAgent.Caspofungin,
            AntifungalAgent.Micafungin,
            AntifungalAgent.Fluconazole,
            AntifungalAgent.Isavuconazole,
            AntifungalAgent.Itraconazole,
            AntifungalAgent.Posaconazole,
            AntifungalAgent.Voriconazole,
            AntifungalAgent.Flucytosine,
            AntifungalAgent.Fluorouracil,
        };

        public int Compare(AntifungalAgent x, AntifungalAgent y) {
            var xIndex = AgentsOrderedByGroup.IndexOf(x);
            var yIndex = AgentsOrderedByGroup.IndexOf(y);
            return xIndex.CompareTo(yIndex);
        }
    };
}