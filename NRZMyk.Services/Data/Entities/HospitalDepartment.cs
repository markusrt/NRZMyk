using System.ComponentModel;

namespace NRZMyk.Services.Data.Entities
{
    public enum HospitalDepartment
    {
        [Description("unbekannt")] Unknown = 0,
        [Description("internistisch")] Internal = 1,
        [Description("chirurgisch")] GeneralSurgery = 2,
        [Description("neurologisch")] Neurology = 3,
        [Description("anästhesiologisch")] Anaesthetics = 4,
        [Description("urologisch")] Urology = 5,
        [Description("gynäkologisch")] Gynaecology = 6,
        [Description("neo/päd")] Pediadric = 7,
        [Description("ophthalmologisch")] Ophthalmology = 8,
        [Description("HNO")] Otorhinolaryngology = 9,
        [Description("dermatologisch")] Dermatology = 10,
        [Description("Andere")] Other = 10000,
    }
}