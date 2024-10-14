namespace NRZMyk.Services.Models.EmailTemplates;

public class RemindOrganizationOnDispatchMonth
{
    public string OrganizationName { get; set; }
    public string DispatchMonth { get; set; }
    public string LatestCryoDate { get; set; }

    public override string ToString()
    {
        return $"Organization: {OrganizationName}, DispatchMonth: {DispatchMonth}, LatestCryoDate: {LatestCryoDate}";
    }
}