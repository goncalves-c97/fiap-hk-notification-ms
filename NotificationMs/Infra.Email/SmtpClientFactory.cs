using Core.Dtos;
using System.Net;
using System.Net.Mail;

namespace Infra.Email
{
    public interface ISmtpClientAdapter : IDisposable
    {
        Task SendMailAsync(MailMessage message);
    }

    public interface ISmtpClientFactory
    {
        ISmtpClientAdapter Create(EmailSettingsDto emailSettings);
    }

    public sealed class SmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpClientAdapter Create(EmailSettingsDto emailSettings)
        {
            var smtpClient = new SmtpClient
            {
                Host = emailSettings.Host,
                Port = emailSettings.Port,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings.Mail, emailSettings.Password)
            };

            return new SmtpClientAdapter(smtpClient);
        }
    }

    public sealed class SmtpClientAdapter(SmtpClient smtpClient) : ISmtpClientAdapter
    {
        public Task SendMailAsync(MailMessage message)
        {
            return smtpClient.SendMailAsync(message);
        }

        public void Dispose()
        {
            smtpClient.Dispose();
        }
    }
}
