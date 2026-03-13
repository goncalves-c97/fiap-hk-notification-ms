using Core.Interfaces;
using Infra.Email;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Infra.Email;

public class InfraEmailBootstrapperTests
{
    [Fact]
    public void Register_ShouldAddEmailServiceAsTransient()
    {
        var services = new ServiceCollection();

        InfraEmailBootstrapper.Register(services);

        var descriptor = Assert.Single(services.Where(service => service.ServiceType == typeof(IEmailService)));
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.Equal(typeof(EmailService), descriptor.ImplementationType);
    }
}
