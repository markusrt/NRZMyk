using brevo_csharp.Model;

namespace NRZMyk.Services.Services
{
    public interface IBrevoEmailClient
    {
        System.Threading.Tasks.Task<CreateSmtpEmail> SendTransacEmailAsync(SendSmtpEmail sendSmtpEmail);
    }
}
