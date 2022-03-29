using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum InternalHospitalDepartmentType
    {
        [Description("-")] NoInternalDepartment = 0,
        [Description("hämatoonkologisch")] HematoOncological = 10,
        [Description("gastroenterologisch")] Gastroenterological = 20,
        [Description("infektiologisch")] Infectiological = 30,
        [Description("pulmologisch")] Pulmological = 40,
        [Description("kardiologisch")] Cardiological = 50,
        [Description("endokrinologisch")] Endocrinological = 60,
        [Description("nephrologisch")] Nephrological = 70,
        [Description("angiologisch")] Angiological = 80,
        [Description("rheumatologisch")] Rheumatological = 90,
        [Description("andere")] Other = 10000,
    }
}