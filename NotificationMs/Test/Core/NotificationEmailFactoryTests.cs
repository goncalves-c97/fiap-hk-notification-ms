using Core.Events;
using Core.Factories;

namespace Test.Core;

public class NotificationEmailFactoryTests
{
    [Fact]
    public void GetNotificationEmailRequestDto_ShouldBuildExpectedEmail()
    {
        var evt = new VideoProcessedEvent
        {
            UserEmail = "user@example.com",
            OriginalVideoName = "video.mp4",
            ProcessedVideoUrl = "https://example.com/video.zip"
        };

        var result = NotificationEmailFactory.GetVideoProcessedNotificationEmailRequestDto(evt);

        Assert.Equal("user@example.com", result.ToEmail);
        Assert.Equal("Vídeo processado com sucesso!", result.Subject);
        Assert.Contains("video.mp4", result.Body);
        Assert.Contains("https://example.com/video.zip", result.Body);
        Assert.Contains(DateTime.Now.Year.ToString(), result.Body);
    }
}
