using Core.Dtos;
using Core.Events;
using Core.Handlers;
using Core.Interfaces;
using Moq;

namespace Test.Core;

public class VideoProcessedNotificationHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldBuildAndSendNotificationEmail()
    {
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var handler = new VideoProcessedNotificationHandler();
        var evt = new VideoProcessedEvent
        {
            UserEmail = "user@example.com",
            OriginalVideoName = "frames.mp4",
            ProcessedVideoUrl = "https://example.com/frames.zip"
        };

        emailService
            .Setup(service => service.SendEmailAsync(It.IsAny<EmailRequestDto>()))
            .Callback<EmailRequestDto>(email =>
            {
                Assert.Equal("user@example.com", email.ToEmail);
                Assert.Contains("frames.mp4", email.Body);
                Assert.Contains("https://example.com/frames.zip", email.Body);
            })
            .Returns(Task.CompletedTask);

        await handler.HandleVideoProcessedSuccessfullyEventAsync(evt, emailService.Object);

        emailService.Verify(service => service.SendEmailAsync(It.IsAny<EmailRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenEmailServiceFails_ShouldRethrow()
    {
        var emailService = new Mock<IEmailService>(MockBehavior.Strict);
        var handler = new VideoProcessedNotificationHandler();
        var evt = new VideoProcessedEvent
        {
            UserEmail = "user@example.com",
            OriginalVideoName = "frames.mp4",
            ProcessedVideoUrl = "https://example.com/frames.zip"
        };

        emailService
            .Setup(service => service.SendEmailAsync(It.IsAny<EmailRequestDto>()))
            .ThrowsAsync(new InvalidOperationException("smtp failed"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleVideoProcessedSuccessfullyEventAsync(evt, emailService.Object));

        Assert.Equal("smtp failed", exception.Message);
    }
}
