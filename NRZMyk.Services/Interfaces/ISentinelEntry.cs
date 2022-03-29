using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Interfaces;

public interface ISentinelEntry
{
    public Material Material { get; set; }
    public string OtherMaterial { get; set; }
    public Species IdentifiedSpecies { get; set; }
    public string OtherIdentifiedSpecies { get; set; }
    public HospitalDepartment HospitalDepartment { get; set; }
    public HospitalDepartmentType HospitalDepartmentType { get; set; }
    public InternalHospitalDepartmentType InternalHospitalDepartmentType { get; set; }
    public string OtherHospitalDepartment { get; set; }
    public SpeciesIdentificationMethod SpeciesIdentificationMethod { get; set; }
    public string PcrDetails { get; set; }
}