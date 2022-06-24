using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Interfaces;

public interface ISentinelEntry
{
    public Material Material { get; set; }
    public string OtherMaterial { get; set; }
    public Species IdentifiedSpecies { get; }
    public string OtherIdentifiedSpecies { get; }
    public HospitalDepartment HospitalDepartment { get; set; }
    public HospitalDepartmentType HospitalDepartmentType { get; set; }
    public InternalHospitalDepartmentType InternalHospitalDepartmentType { get; set; }
    public string OtherHospitalDepartment { get; set; }
    public SpeciesIdentificationMethod SpeciesIdentificationMethod { get; }
    public string PcrDetails { get; }
}