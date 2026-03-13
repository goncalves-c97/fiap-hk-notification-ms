using Core.Events;
using Core.Interfaces;

namespace NotificationSenderWorker
{
    public class NotificationWorker : BackgroundService
    {
        private readonly IMessagingService _consumer;
        private readonly IVideoProcessedNotificationHandler _handler;
        private readonly IEmailService _emailService; 

        public NotificationWorker(
            IMessagingService consumer,
            IVideoProcessedNotificationHandler handler,
            IEmailService emailService)
        {
            _consumer = consumer;
            _handler = handler;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Iniciando NotificationWorker...");

            _consumer.Subscribe<VideoProcessedEvent>(
                "video-processed",
                async evt =>
                {
                    await _handler.HandleVideoProcessedSuccessfullyEventAsync(evt, _emailService);
                });

            _consumer.Subscribe<VideoProcessedEvent>(
                "processing-error",
                async evt =>
                {
                    await _handler.HandleVideoProcessingErrorEventAsync(evt, _emailService);
                });

            Console.WriteLine("NotificationWorker iniciado e aguardando mensagens...");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
