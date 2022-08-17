using System.Net;
using System.Net.Mail;

namespace Blog.Services
{
    public class EmailService
    {
        public bool Send(
        string toName,
        string toEmail,
        string subject,
        string body,
        string fromName = "Equipe balta.io",
        string fromEmail = "joaovitorsoareslima12@gmail.com")
        {
            var smtpClient = new SmtpClient(Configuration.smtpConfiguration.Host, Configuration.smtpConfiguration.Port);

            smtpClient.Credentials = new NetworkCredential(Configuration.smtpConfiguration.UserName, Configuration.smtpConfiguration.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            var mail = new MailMessage();

            mail.From = new MailAddress(fromEmail, fromName);
            mail.To.Add(new MailAddress(toEmail, toName));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
