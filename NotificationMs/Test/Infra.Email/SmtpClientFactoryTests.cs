using Core.Dtos;
using Infra.Email;
using System.Net.Mail;

namespace Test.Infra.Email;

public class SmtpClientFactoryTests
{
    [Fact]
    public void Create_ShouldReturnDisposableAdapter()
    {
        var factory = new SmtpClientFactory();
        var settings = new EmailSettingsDto
        {
            Mail = "sender@example.com",
            DisplayName = "Sender",
            Password = "secret",
            Host = "smtp.example.com",
            Port = 2525
        };

        using var adapter = factory.Create(settings);

        Assert.IsType<SmtpClientAdapter>(adapter);
    }

    [Fact]
    public async Task SendMailAsync_WithPickupDirectoryClient_ShouldCreateMailFile()
    {
        var pickupDirectory = Path.Combine(Path.GetTempPath(), $"smtp-pickup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(pickupDirectory);

        try
        {
            using var smtpClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = pickupDirectory
            };
            using var adapter = new SmtpClientAdapter(smtpClient);
            using var message = new MailMessage("sender@example.com", "target@example.com", "Subject", "Body");

            await adapter.SendMailAsync(message);

            Assert.Single(Directory.GetFiles(pickupDirectory));
        }
        finally
        {
            Directory.Delete(pickupDirectory, recursive: true);
        }
    }
}
