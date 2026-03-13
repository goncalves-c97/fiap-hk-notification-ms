using Core.Events;
using Core.Factories;
using Core.Enums;

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

    [Theory]
    [InlineData(StatusVideoEnum.NotFound, "Vídeo não encontrado!", "não conseguimos encontrar o seu vídeo")]
    [InlineData(StatusVideoEnum.Error, "Estamos tentando processar o seu vídeo!", "vamos continuar tentando processá-lo")]
    [InlineData(StatusVideoEnum.ErrorAttemptsExceeded, "Houve um erro no processamento do seu vídeo!", "após várias tentativas")]
    [InlineData(StatusVideoEnum.Pending, "Houve um erro no processamento do seu vídeo!", "não conseguimos processar o seu vídeo")]
    public void GetVideoProcessingErrorNotificationEmailRequestDto_ShouldBuildExpectedEmailForStatus(
        StatusVideoEnum status,
        string expectedSubject,
        string expectedMessageFragment)
    {
        var evt = new VideoProcessedEvent
        {
            UserEmail = "user@example.com",
            OriginalVideoName = "video.mp4",
            StatusVideoEnum = status
        };

        var result = NotificationEmailFactory.GetVideoProcessingErrorNotificationEmailRequestDto(evt);

        Assert.Equal("user@example.com", result.ToEmail);
        Assert.Equal(expectedSubject, result.Subject);
        Assert.Contains("video.mp4", result.Body);
        Assert.Contains(expectedMessageFragment, result.Body);
        Assert.Contains(DateTime.Now.Year.ToString(), result.Body);
    }
}
