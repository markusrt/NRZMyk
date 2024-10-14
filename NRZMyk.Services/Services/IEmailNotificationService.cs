using System.Threading.Tasks;
using NRZMyk.Services.Data.Entities;

namespace NRZMyk.Services.Services
{
    public interface IEmailNotificationService
    {
        Task NotifyNewUserRegistered(string userName, string userEmail, string userCity);
        Task RemindOrganizationOnDispatchMonth(Organization organization);
    }
}