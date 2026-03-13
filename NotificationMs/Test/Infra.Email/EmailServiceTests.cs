using Core.Dtos;
using Infra.Email;
using Infra.Email.Exceptions;
using Moq;
using System.Net.Mail;

namespace Test.Infra.Email;

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_ShouldBuildMessageAndSendUsingFactoryClient()
    {
        var settings = CreateSettings();
        var smtpClient = new Mock<ISmtpClientAdapter>(MockBehavior.Strict);
        var smtpFactory = new Mock<ISmtpClientFactory>(MockBehavior.Strict);
        MailMessage? sentMessage = null;

        smtpFactory
            .Setup(factory => factory.Create(settings))
            .Returns(smtpClient.Object);

        smtpClient
            .Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
            .Callback<MailMessage>(message => sentMessage = message)
            .Returns(Task.CompletedTask);

        smtpClient.Setup(client => client.Dispose());

        var service = new EmailService(settings, smtpFactory.Object);

        await service.SendEmailAsync(new EmailRequestDto
        {
            ToEmail = "target@example.com",
            Subject = "Subject",
            Body = "<b>Hello</b>"
        });

        smtpFactory.Verify(factory => factory.Create(settings), Times.Once);
        smtpClient.Verify(client => client.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);
        smtpClient.Verify(client => client.Dispose(), Times.Once);

        Assert.NotNull(sentMessage);
        var message = sentMessage!;

        Assert.Equal("sender@example.com", message.From!.Address);
        Assert.Equal("Target Display", message.From.DisplayName);
        Assert.Equal("target@example.com", message.To.Single().Address);
        Assert.Equal("Subject", message.Subject);
        Assert.Equal("<b>Hello</b>", message.Body);
        Assert.True(message.IsBodyHtml);
    }

    [Fact]
    public async Task SendEmailAsync_WhenMessageBuildFails_ShouldThrowEmailBuildException()
    {
        var invalidSettings = new EmailSettingsDto
        {
            Mail = "invalid-mail-address",
            DisplayName = "Target Display",
            Password = "secret",
            Host = "smtp.example.com",
            Port = 25
        };

        var service = new EmailService(invalidSettings);

        var exception = await Assert.ThrowsAsync<EmailBuildException>(() => service.SendEmailAsync(new EmailRequestDto
        {
            ToEmail = "target@example.com",
            Subject = "Subject",
            Body = "Hello"
        }));

        Assert.Equal("Falha na montagem do e-mail", exception.Message);
        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task SendEmailAsync_WhenSendFails_ShouldThrowEmailFailureException()
    {
        var settings = CreateSettings();
        var smtpClient = new Mock<ISmtpClientAdapter>(MockBehavior.Strict);
        var smtpFactory = new Mock<ISmtpClientFactory>(MockBehavior.Strict);

        smtpFactory
            .Setup(factory => factory.Create(settings))
            .Returns(smtpClient.Object);

        smtpClient
            .Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
            .ThrowsAsync(new SmtpException("connection failed"));

        smtpClient.Setup(client => client.Dispose());

        var service = new EmailService(settings, smtpFactory.Object);

        var exception = await Assert.ThrowsAsync<EmailFailureException>(() => service.SendEmailAsync(new EmailRequestDto
        {
            ToEmail = "target@example.com",
            Subject = "Subject",
            Body = "Hello"
        }));

        Assert.Equal("Falha no envio do e-mail", exception.Message);
        Assert.IsType<SmtpException>(exception.InnerException);
        smtpClient.Verify(client => client.Dispose(), Times.Once);
    }

    private static EmailSettingsDto CreateSettings()
    {
        return new EmailSettingsDto
        {
            Mail = "sender@example.com",
            DisplayName = "Target Display",
            Password = "secret",
            Host = "smtp.example.com",
            Port = 2525
        };
    }
}
