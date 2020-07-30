using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum SpeciesTestingMethod
    {
        Vitek,
        [Description("Mikrodilution")]
        Microdilution,
        [Description("Yeast One")]
        YeastOne,
        Micronaut,
        [Description("E-Test")]
        ETest
    }
}