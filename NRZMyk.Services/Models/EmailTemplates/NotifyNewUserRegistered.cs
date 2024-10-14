namespace NRZMyk.Services.Models.EmailTemplates
{
    public class NotifyNewUserRegistered
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserCity { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        
        public override string ToString()
        {
            return $"UserName: {UserName}, UserCity: {UserCity}, Date: {Date}, Time: {Time}";
        }
    }
}