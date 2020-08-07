using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum BrothMicrodilutionStandard
    {
        None = 0,
        [Description("EUCAST")]
        Eucast = 1,
        [Description("CSLI")]
        Csli = 2
    }
}