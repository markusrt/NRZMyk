using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum SpeciesIdentificationMethod
    {
        [Description("Keine")] None = 0,
        [Description("MALDI-TOF")] MaldiTof = 100,
        [Description("PCR")] Pcr = 200,
    }
}