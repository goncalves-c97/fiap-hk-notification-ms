using Core.Events;
using Core.Interfaces;
using Moq;

namespace Test.NotificationWorker;

public class NotificationWorkerTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldSubscribeToQueuesAndDelegateSuccessEvents()
    {
        var consumer = new Mock<IMessagingService>(MockBehavior.Strict);
        var handler = new Mock<IVideoProcessedNotificationHandler>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var successEvent = CreateEvent("success@example.com");
        Func<VideoProcessedEvent, Task>? successSubscriptionHandler = null;
        Func<VideoProcessedEvent, Task>? errorSubscriptionHandler = null;

        consumer
            .Setup(service => service.Subscribe("video-processed", It.IsAny<Func<VideoProcessedEvent, Task>>()))
            .Callback<string, Func<VideoProcessedEvent, Task>>((_, callback) => successSubscriptionHandler = callback);

        consumer
            .Setup(service => service.Subscribe("processing-error", It.IsAny<Func<VideoProcessedEvent, Task>>()))
            .Callback<string, Func<VideoProcessedEvent, Task>>((_, callback) => errorSubscriptionHandler = callback);

        handler
            .Setup(service => service.HandleVideoProcessedSuccessfullyEventAsync(successEvent, emailService.Object, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var worker = new TestableNotificationWorker(consumer.Object, handler.Object, emailService.Object);

        using var cancellation = new CancellationTokenSource();
        var executionTask = worker.RunExecuteAsync(cancellation.Token);

        Assert.NotNull(successSubscriptionHandler);
        Assert.NotNull(errorSubscriptionHandler);

        await successSubscriptionHandler!(successEvent);

        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => executionTask);

        consumer.Verify(service => service.Subscribe("video-processed", It.IsAny<Func<VideoProcessedEvent, Task>>()), Times.Once);
        consumer.Verify(service => service.Subscribe("processing-error", It.IsAny<Func<VideoProcessedEvent, Task>>()), Times.Once);
        handler.Verify(service => service.HandleVideoProcessedSuccessfullyEventAsync(successEvent, emailService.Object, It.IsAny<CancellationToken>()), Times.Once);
        handler.Verify(service => service.HandleVideoProcessingErrorEventAsync(It.IsAny<VideoProcessedEvent>(), It.IsAny<IEmailService>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldDelegateErrorEvents()
    {
        var consumer = new Mock<IMessagingService>(MockBehavior.Strict);
        var handler = new Mock<IVideoProcessedNotificationHandler>(MockBehavior.Strict);
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var errorEvent = CreateEvent("error@example.com");
        Func<VideoProcessedEvent, Task>? successSubscriptionHandler = null;
        Func<VideoProcessedEvent, Task>? errorSubscriptionHandler = null;

        consumer
            .Setup(service => service.Subscribe("video-processed", It.IsAny<Func<VideoProcessedEvent, Task>>()))
            .Callback<string, Func<VideoProcessedEvent, Task>>((_, callback) => successSubscriptionHandler = callback);

        consumer
            .Setup(service => service.Subscribe("processing-error", It.IsAny<Func<VideoProcessedEvent, Task>>()))
            .Callback<string, Func<VideoProcessedEvent, Task>>((_, callback) => errorSubscriptionHandler = callback);

        handler
            .Setup(service => service.HandleVideoProcessingErrorEventAsync(errorEvent, emailService.Object, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var worker = new TestableNotificationWorker(consumer.Object, handler.Object, emailService.Object);

        using var cancellation = new CancellationTokenSource();
        var executionTask = worker.RunExecuteAsync(cancellation.Token);

        Assert.NotNull(successSubscriptionHandler);
        Assert.NotNull(errorSubscriptionHandler);

        await errorSubscriptionHandler!(errorEvent);

        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => executionTask);

        handler.Verify(service => service.HandleVideoProcessingErrorEventAsync(errorEvent, emailService.Object, It.IsAny<CancellationToken>()), Times.Once);
        handler.Verify(service => service.HandleVideoProcessedSuccessfullyEventAsync(It.IsAny<VideoProcessedEvent>(), It.IsAny<IEmailService>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static VideoProcessedEvent CreateEvent(string email)
    {
        return new VideoProcessedEvent
        {
            UserEmail = email,
            OriginalVideoName = "video.mp4",
            ProcessedVideoUrl = "https://example.com/video.zip"
        };
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
