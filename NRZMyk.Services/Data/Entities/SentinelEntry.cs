using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NRZMyk.Services.Interfaces;

namespace NRZMyk.Services.Data.Entities
{
    public class SentinelEntry : BaseEntity, IAggregateRoot, ISentinelEntry
    {
        [Display(Name = "Entnahmedatum")]
        public DateTime? SamplingDate { get; set; }

        [Display(Name = "Eingangsdatum")]
        public DateTime? ReceivingDate { get; set; }

        [Display(Name = "Labnr. Einsender")]
        public string SenderLaboratoryNumber { get; set; }
        
        [Display(Name = "Material")]
        public Material Material { get; set; }
        public string OtherMaterial { get; set; }

        [Display(Name = "Stationstyp")]
        public HospitalDepartmentType HospitalDepartmentType { get; set; }

        [Display(Name = "Stationstyp")]
        public InternalHospitalDepartmentType InternalHospitalDepartmentType { get; set; }

        [Display(Name = "Station")]
        public HospitalDepartment HospitalDepartment { get; set; }
        
        public string OtherHospitalDepartment { get; set; }

        [Display(Name = "Methode Speziesidentifikation")]
        public SpeciesIdentificationMethod SpeciesIdentificationMethod { get; set; }
        public string PcrDetails { get; set; }

        [Display(Name = "Spezies")]
        public Species IdentifiedSpecies { get; set; }
        public string OtherIdentifiedSpecies { get; set; }

        [Display(Name = "Altersgruppe")]
        public AgeGroup AgeGroup { get; set; }

        [Display(Name = "Bemerkung")]
        public string Remark { get; set; }

        [Display(Name = "Geschlecht")]
        public Gender Gender { get; set; }

        public int CryoBoxNumber { get; set; }
        
        public int CryoBoxSlot { get; set; }

        public int YearlySequentialEntryNumber { get; set; }

        public int Year { get; set; }

        public ICollection<AntimicrobialSensitivityTest> AntimicrobialSensitivityTests { get; set; }

        [Display(Name = "Kryo-Box")]
        public string CryoBox => $"SN-{CryoBoxNumber:0000}";
        
        [Display(Name = "Labornummer")]
        public string LaboratoryNumber => $"SN-{Year}-{YearlySequentialEntryNumber:0000}";

        public string ProtectKey { get; set; }

        [Display(Name = "Kryo-Datum")]
        public DateTime? CryoDate { get; set; }

        [Display(Name = "Kryo-Kommentar")]
        public string CryoRemark { get; set; }

        public YesNo HasPredecessor => PredecessorEntryId.HasValue ? YesNo.Yes : YesNo.No;

        public string PredecessorLaboratoryNumber => PredecessorEntry?.LaboratoryNumber;
        
        public int? PredecessorEntryId {get;set;}

        [JsonIgnore]
        public virtual SentinelEntry PredecessorEntry {get;set;}
    }
}