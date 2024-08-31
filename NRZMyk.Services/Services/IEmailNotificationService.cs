using System.Threading.Tasks;

namespace NRZMyk.Services.Services
{
    public interface IEmailNotificationService
    {
        Task NotifyNewUserRegistered(string userName, string userEmail, string userCity);
        Task SendEmail(string email, string message);
    }
}