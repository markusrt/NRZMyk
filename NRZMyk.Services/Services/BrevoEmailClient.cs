using brevo_csharp.Api;
using brevo_csharp.Model;

namespace NRZMyk.Services.Services
{
    public class BrevoEmailClient : IBrevoEmailClient
    {
        private readonly TransactionalEmailsApi _api;

        public BrevoEmailClient(TransactionalEmailsApi api)
        {
            _api = api;
        }

        public async System.Threading.Tasks.Task<CreateSmtpEmail> SendTransacEmailAsync(SendSmtpEmail sendSmtpEmail)
        {
            return await _api.SendTransacEmailAsync(sendSmtpEmail);
        }
    }
}
