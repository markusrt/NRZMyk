using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum SpeciesIdentificationMethod
    {
        [Description("-")] None = 0,
        [Description("MALDI-TOF")] MaldiTof = 100,
        [Description("PCR")] Pcr = 200,
        [Description("Andere")] Other = 300
    }
}