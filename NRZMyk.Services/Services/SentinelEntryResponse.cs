using System;

namespace NRZMyk.Services.Services;

public class SentinelEntryResponse : SentinelEntryRequest
{
    public string LaboratoryNumber { get; set; }

    public string CryoBox { get; set; }

    public DateTime? CryoDate { get; set; }

    public string CryoRemark { get; set; }
}