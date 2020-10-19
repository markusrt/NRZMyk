using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum HospitalDepartmentType
    {
        [Description("unbekannt")] Unknown = 0,
        [Description("ITS")] IntensiveCareUnit = 1,
        [Description("NS")] NormalUnit = 2
    }
}