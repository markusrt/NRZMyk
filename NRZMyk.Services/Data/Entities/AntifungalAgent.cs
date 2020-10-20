using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum AntifungalAgent
    {
        Unknown=0,
        [Description("Amphotericin B")]
        AmphotericinB=100,
        Anidulafungin=200,
        Caspofungin=300,
        [Description("Fluconazol")]
        Fluconazole=400,
        [Description("5-FC")]
        Flucytosine,
        [Description("5-FU")]
        Fluorouracil=500,
        [Description("Isavuconazol")]
        Isavuconazole=600,
        [Description("Itraconazol")]
        Itraconazole=700,
        Micafungin=800,
        [Description("Posaconazol")]
        Posaconazole=900,
        [Description("Voriconazol")]
        Voriconazole=1000,
        [Description("Andere")]
        Other=10000
    }
}