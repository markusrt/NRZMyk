using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Utils;
using NUnit.Framework;

namespace NRZMyk.Services.Tests.Utils
{
    public class AntifungalAgentComparerTests
    {
        [Test]
        public void WhenUnsorted_SortsByGroups()
        {
            var sut = new AntifungalAgentComparer();
            var antifungalAgents = new List<AntifungalAgent>
            {
                AntifungalAgent.Voriconazole,
                AntifungalAgent.Anidulafungin,
                AntifungalAgent.Micafungin,
                AntifungalAgent.Fluconazole,
                AntifungalAgent.Isavuconazole,
                AntifungalAgent.Caspofungin,
                AntifungalAgent.Itraconazole,
                AntifungalAgent.Flucytosine,
                AntifungalAgent.AmphotericinB,
                AntifungalAgent.Fluorouracil,
                AntifungalAgent.Posaconazole,
            };

            antifungalAgents.Sort(sut);

            antifungalAgents[0].Should().Be(AntifungalAgent.AmphotericinB);
            antifungalAgents[1].Should().Be(AntifungalAgent.Anidulafungin);
            antifungalAgents[2].Should().Be(AntifungalAgent.Caspofungin);
            antifungalAgents[3].Should().Be(AntifungalAgent.Micafungin);
            antifungalAgents[4].Should().Be(AntifungalAgent.Fluconazole);
            antifungalAgents[5].Should().Be(AntifungalAgent.Isavuconazole);
            antifungalAgents[6].Should().Be(AntifungalAgent.Itraconazole);
            antifungalAgents[7].Should().Be(AntifungalAgent.Posaconazole);
            antifungalAgents[8].Should().Be(AntifungalAgent.Voriconazole);
            antifungalAgents[9].Should().Be(AntifungalAgent.Flucytosine);
            antifungalAgents[10].Should().Be(AntifungalAgent.Fluorouracil);
        }
    }
}