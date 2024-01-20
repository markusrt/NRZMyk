using System.Threading.Tasks;

namespace NRZMyk.Services.Services
{
    public interface IEmailNotificationService
    {
        Task NotifyNewUserRegistered(string userName, string userEmail, string userCity);
        void SendEmail(string email, string message);
    }
}