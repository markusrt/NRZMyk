using System.Collections.Generic;

namespace NRZMyk.Services.Configuration
{
    public class Application
    {
        public int CryoBoxSize { get; set; }
        public List<string> OtherSpecies { get; set; }
        public string SendGridSenderEmail { get; set; }
        public string SendGridDynamicTemplateId { get; set; }
        public string AdministratorEmail { get; set; }
        public JobSetting SentinelReminderJob { get; set; }
    }
}