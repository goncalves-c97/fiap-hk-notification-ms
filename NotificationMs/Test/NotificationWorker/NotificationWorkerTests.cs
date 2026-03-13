using Core.Events;
using Core.Interfaces;
using Moq;
using NotificationSenderWorker;

namespace Test.NotificationWorker;

public class NotificationWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldSubscribeToQueueAndDelegateEventsToHandler()
    {
        var consumer = new Mock<IMessagingService>(MockBehavior.Strict);
        var handler = new Mock<IVideoProcessedNotificationHandler>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var evt = new VideoProcessedEvent
        {
            UserEmail = "user@example.com",
            OriginalVideoName = "video.mp4",
            ProcessedVideoUrl = "https://example.com/video.zip"
        };

        Func<VideoProcessedEvent, Task>? subscriptionHandler = null;

        consumer
            .Setup(service => service.Subscribe("video-processed", It.IsAny<Func<VideoProcessedEvent, Task>>()))
            .Callback<string, Func<VideoProcessedEvent, Task>>((_, callback) => subscriptionHandler = callback);

        handler
            .Setup(service => service.HandleVideoProcessedSuccessfullyEventAsync(evt, emailService.Object, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var worker = new TestableNotificationWorker(consumer.Object, handler.Object, emailService.Object);

        using var cancellation = new CancellationTokenSource();
        var executionTask = worker.RunExecuteAsync(cancellation.Token);

        Assert.NotNull(subscriptionHandler);

        await subscriptionHandler!(evt);

        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => executionTask);

        consumer.Verify(service => service.Subscribe("video-processed", It.IsAny<Func<VideoProcessedEvent, Task>>()), Times.Once);
        handler.Verify(service => service.HandleVideoProcessedSuccessfullyEventAsync(evt, emailService.Object, It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed class TestableNotificationWorker(
        IMessagingService consumer,
        IVideoProcessedNotificationHandler handler,
        IEmailService emailService) : NotificationSenderWorker.NotificationWorker(consumer, handler, emailService)
    {
        public Task RunExecuteAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync(cancellationToken);
        }
    }
}
