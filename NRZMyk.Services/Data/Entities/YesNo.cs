using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum YesNo
    {
        [Description("Nein")]
        No = 0,
        [Description("Ja")]
        Yes = 1,
    }
}