using NRZMyk.Services.Services;

namespace NRZMyk.Client.Services;

public class ClientEmailNotificationService : IEmailNotificationService
{
    public Task NotifyNewUserRegistered(string userName, string userEmail, string userCity)
    {
        return Task.CompletedTask;
    }

    public void SendEmail(string email, string message)
    {
    }
}