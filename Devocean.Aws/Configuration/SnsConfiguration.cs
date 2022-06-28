using Amazon.SimpleNotificationService;
using Devocean.Aws.Sns.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devocean.Aws.Configuration;

public static class SnsConfiguration
{
    public static void AddAwsSnsServices(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.AddScoped<ListTopics>();
        services.AddScoped<SendSmsMessage>();
    }
}
