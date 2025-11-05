using System.Threading.Tasks;
using brevo_csharp.Model;

namespace NRZMyk.Services.Services
{
    public interface IBrevoEmailClient
    {
        Task<CreateSmtpEmail> SendTransacEmailAsync(SendSmtpEmail sendSmtpEmail);
    }
}
