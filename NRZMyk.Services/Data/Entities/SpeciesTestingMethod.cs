using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum SpeciesTestingMethod
    {
        Vitek,
        [Description("Mikrodilution")]
        Microdilution,
        YeastOne,
        Micronaut,
        ETest
    }
}