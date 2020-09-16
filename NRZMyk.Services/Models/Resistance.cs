using System.ComponentModel;

namespace NRZMyk.Services.Models
{
    public enum Resistance
    {
        [Description("sensibel")] Susceptible,
        [Description("intermediär")] Intermediate,
        [Description("resistent")] Resistant,
        [Description("n.d.")] NotDetermined
    }
}