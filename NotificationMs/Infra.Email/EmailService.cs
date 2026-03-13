using Core.Dtos;
using Core.Interfaces;
using Infra.Email.Exceptions;
using System.Net.Mail;

namespace Infra.Email
{
    public class EmailService(EmailSettingsDto emailSettings, ISmtpClientFactory? smtpClientFactory = null) : IEmailService
    {
        private readonly ISmtpClientFactory _smtpClientFactory = smtpClientFactory ?? new SmtpClientFactory();

        public async Task SendEmailAsync(EmailRequestDto emailRequestDto)
        {
            MailMessage messageObj;
            ISmtpClientAdapter smtpClient;

            try
            {
                messageObj = new MailMessage()
                {
                    From = new MailAddress(emailSettings.Mail, emailSettings.DisplayName),
                    Subject = emailRequestDto.Subject,
                    IsBodyHtml = true,
                    Body = emailRequestDto.Body
                };

                messageObj.To.Add(emailRequestDto.ToEmail);

                smtpClient = _smtpClientFactory.Create(emailSettings);
            }
            catch (Exception ex)
            {
                throw new EmailBuildException("Falha na montagem do e-mail", ex);
            }

            try
            {
                using (smtpClient)
                {
                    await smtpClient.SendMailAsync(messageObj);
                }
            }
            catch (Exception ex)
            {
                throw new EmailFailureException("Falha no envio do e-mail", ex);
            }
        }
    }
}
