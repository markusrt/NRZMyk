using System.Collections.Generic;

namespace NRZMyk.Services.Configuration
{
    public class Application
    {
        public int CryoBoxSize { get; set; }
        public List<string> OtherSpecies { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public long NewUserRegisteredTemplateId { get; set; }
        public long RemindOrganizationOnDispatchMonthTemplateId { get; set; }
        public string AdministratorEmail { get; set; }
        public JobSetting SentinelReminderJob { get; set; }
    }
}