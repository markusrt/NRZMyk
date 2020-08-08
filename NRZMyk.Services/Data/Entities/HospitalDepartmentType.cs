using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum HospitalDepartmentType
    {
        [Description("Unbekannt")] Unknown = 0,
        [Description("I T S")] IntensiveCareUnit = 1,
        [Description("N S")] NormalUnit = 2
    }
}