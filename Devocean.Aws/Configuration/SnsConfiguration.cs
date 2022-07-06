using Amazon.SimpleNotificationService;
using Devocean.Aws.Sns.UseCases;
using Devocean.Core.Application.UseCases.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Devocean.Aws.Configuration;

public static class SnsConfiguration
{
    public static void AddAwsSnsServices(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSimpleNotificationService>();
        services.AddScoped<HandlerBase<ListTopics, ListTopics.Response>>();
        services.AddScoped<HandlerBase<SendSmsMessage, SendSmsMessage.Response>>();
    }
}
