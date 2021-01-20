using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum HospitalDepartmentType
    {
        [Description("unbekannt")] Unknown = 0,
        [Description("Intensivstation")] IntensiveCareUnit = 1,
        [Description("Normalstation")] NormalUnit = 2,
        [Description("Ambulanz")] OutpatientsUnit = 3
    }
}