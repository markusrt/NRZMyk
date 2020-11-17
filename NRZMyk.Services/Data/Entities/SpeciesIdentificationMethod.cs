using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum SpeciesIdentificationMethod
    {
        [Description("-")] None = 0,
        [Description("API (bioMérieux)")] API = 10,
        [Description("BBL Crystal (Becton-Dickinson)")] BBL = 20,
        [Description("MALDI-TOF")] MaldiTof = 100,
        [Description("MicroScan (Siemens)")] MicroScan = 150,
        [Description("PCR")] Pcr = 200,
        [Description("Phoenix (Becton-Dickinson)")] Phoenix = 205,
        [Description("RapID System (Remel)")] RapID = 225,
        [Description("Sensititre (Trek Diagnostics)")] Sensititre = 235,
        [Description("Vitek (bioMérieux)")] Vitek = 250,
        [Description("Andere")] Other = 300
    }
}