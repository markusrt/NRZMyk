using System.ComponentModel;

namespace NRZMyk.Services.Models
{
    public enum Resistance
    {
        [Description("sensibel")] Susceptible = 0,
        [Description("intermediär")] Intermediate = 1,
        [Description("resistent")] Resistant = 2,
        [Description("n.d.")] NotDetermined = 3
    }
}