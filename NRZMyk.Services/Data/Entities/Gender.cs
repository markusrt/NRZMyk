using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum Gender
    {
        [Description("keine Angabe")]
        NotStated = 0,
        [Description("männlich")]
        Male=1,
        [Description("weiblich")]
        Female=2,
        [Description("divers")]
        Intersex = 3

    }
}