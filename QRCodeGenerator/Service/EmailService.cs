using QRCodeGenerator.Model;

namespace QRCodeGenerator.Service
{
    public interface IEmailService
    {
        Email SelectedEmail { get; set; }
    }

    public class EmailService : IEmailService
    {
        public Email SelectedEmail { get; set; }
    }
}