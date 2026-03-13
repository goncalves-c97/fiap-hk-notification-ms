using Core.Dtos;
using Core.Handlers;
using Core.Interfaces;
using Infra.Email;
using Infra.Messaging;
using Microsoft.Extensions.Options;
using NotificationSenderWorker;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["MESSAGING:HOST"],
        UserName = builder.Configuration["MESSAGING:USER"],
        Password = builder.Configuration["MESSAGING:PASSWORD"]
    };

    return factory.CreateConnection();
});

builder.Services.AddScoped<IMessagingService, RabbitMqEventBus>();

builder.Services.AddSingleton<IVideoProcessedNotificationHandler, VideoProcessedNotificationHandler>();

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<EmailSettingsDto>(builder.Configuration.GetSection("EMAIL_SETTINGS"));

builder.Services.AddScoped<IEmailService>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<EmailSettingsDto>>().Value;
    return new EmailService(settings);
});

builder.Services.AddHostedService<NotificationWorker>();

var host = builder.Build();

host.Run();