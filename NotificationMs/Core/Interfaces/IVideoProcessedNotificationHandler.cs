using Core.Events;

namespace Core.Interfaces
{
    public interface IVideoProcessedNotificationHandler
    {
        Task HandleVideoProcessedSuccessfullyEventAsync(VideoProcessedEvent videoEvent, IEmailService emailService, CancellationToken cancellationToken = default);
        Task HandleVideoProcessingErrorEventAsync(VideoProcessedEvent videoEvent, IEmailService emailService, CancellationToken cancellationToken = default);
    }
}
