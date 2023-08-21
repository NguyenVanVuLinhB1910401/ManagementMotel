using ManagementMotel.Models;

namespace ManagementMotel.UtilityServices
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
