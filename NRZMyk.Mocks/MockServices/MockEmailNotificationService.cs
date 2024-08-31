using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NRZMyk.Services.Services;

namespace NRZMyk.Mocks.MockServices
{
    public class MockEmailNotificationService : IEmailNotificationService
    {
        public Task NotifyNewUserRegistered(string userName, string userEmail, string userCity)
        {
            return Task.CompletedTask;
        }

        public Task SendEmail(string email, string message)
        {
            return Task.CompletedTask;
        }
    }
}