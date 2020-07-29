using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum ResidentialTreatment
    {
        [Description("Unbekannt")] Unknown = 0,
        [Description("Chir. Intensivstation")] 	SurgicalIntensiveCareUnit = 1,
        [Description("Int. Intensivstation")] InternalIntensiveCareUnit = 2,
        [Description("Gemischte Intensivstation")] MixedIntensiveCareUnit = 3,
        [Description("Int. Normalstation")] InternalNormalUnit = 4,
        [Description("Chir. Normalstation")] SurgicalNormalUnit = 5,
    }
}