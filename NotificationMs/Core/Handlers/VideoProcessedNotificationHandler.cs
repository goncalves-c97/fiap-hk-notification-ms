using Core.Dtos;
using Core.Events;
using Core.Factories;
using Core.Interfaces;

namespace Core.Handlers
{
    public class VideoProcessedNotificationHandler : IVideoProcessedNotificationHandler
    {
        public async Task HandleVideoProcessedSuccessfullyEventAsync(VideoProcessedEvent evt, IEmailService emailService, CancellationToken cancellationToken = default)
        {

            try
            {
                Console.WriteLine($"Preparando notificação via email para {evt.UserEmail}, em relação ao vídeo {evt.OriginalVideoName}");

                EmailRequestDto emailRequestDto = NotificationEmailFactory.GetVideoProcessedNotificationEmailRequestDto(evt);

                await emailService.SendEmailAsync(emailRequestDto);

                Console.WriteLine($"Email de notificação enviado para {evt.UserEmail}, em relação ao vídeo {evt.OriginalVideoName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao notificar usuário a partir do email {evt.UserEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task HandleVideoProcessingErrorEventAsync(VideoProcessedEvent evt, IEmailService emailService, CancellationToken cancellationToken = default)
        {
            try
            {
                Console.WriteLine($"Preparando notificação de erro via email para {evt.UserEmail}.");

                EmailRequestDto emailRequestDto = NotificationEmailFactory.GetVideoProcessingErrorNotificationEmailRequestDto(evt);

                await emailService.SendEmailAsync(emailRequestDto);

                Console.WriteLine($"Email de notificação enviado para {evt.UserEmail}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao notificar usuário a partir do email {evt.UserEmail}: {ex.Message}");
                throw;
            }
        }
    }
}
