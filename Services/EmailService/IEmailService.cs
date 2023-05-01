using BloodNet.Models;

namespace BloodNet.Services.EmailService
{
    public interface IEmailService
    {
        void SendRegisterEmail(EmailDTO request);
    }
}
