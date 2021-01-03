using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum Material
    {
        [Description("-")] None = 0,
        [Description("Blutkultur peripher")]
        PeripheralBloodCulture = 50,
        [Description("Blutkultur zentral - ZVK")]
        CentralBloodCultureCvc = 100,
        [Description("Blutkultur zentral - Port")]
        CentralBloodCulturePort = 200,
        [Description("Blutkultur zentral - Shaldon")]
        CentralBloodCultureShaldon = 300,
        [Description("Blutkultur zentral - k.A.")]
        CentralBloodCultureOther = 400,
        [Description("Blutkultur - ohne genauere Angabe")]
        BloodCultureOther = 500,
        [Description("Andere")] Other = 10000
    }
}