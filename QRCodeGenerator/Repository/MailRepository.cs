using QRCodeGenerator.Model;
using System.Collections.Generic;
using System.Net.Mail;

namespace QRCodeGenerator.Repository
{
    public interface IMailRepository
    {
        IList<SMTPMailServer> GetAll();
    }

    public class MailRepository : IMailRepository
    {
        public IList<SMTPMailServer> GetAll()
        {
            return new List<SMTPMailServer>
            {
              new  SMTPMailServer{
                Provider = "Gmail",
                SMTPSettings = "smtp.gmail.com",
                URL = "gmail.com",
                EnableSsl = true,
                Port = 587,
                SmtpDeliveryMethod = SmtpDeliveryMethod.Network,
                HelpPage = "https://support.google.com/a/answer/176600"
              },
              new  SMTPMailServer{
                Provider = "Outlook.com (former Hotmail)",
                SMTPSettings = "smtp.live.com",
                URL = "Outlook.com"
              },
              new  SMTPMailServer{
                Provider = "Mandic",
                SMTPSettings = "smtp.mandic.com.br",
                URL = "Mandic.com",
                Port = 587,
                EnableSsl = true,
                SmtpDeliveryMethod = SmtpDeliveryMethod.Network,
                HelpPage = "https://wiki.mandic.com.br/e-mail/mandic-mail/gerenciadores-email/configurando-uma-conta-mandic-mail-no-microsoft-outlook-2016-com-protocolo-imap"
              },
              new  SMTPMailServer{
                Provider = "Custom"
              },
            };
        }
    }
}