using System.Security.Claims;
using Devocean.Core.Infrastructure.Services.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace Devocean.Core.Infrastructure.Services;

public static partial class ServiceCollectionExtensions
{
    public static void AddAuthenticationControls(this IServiceCollection services)
    {
        services.AddScopedFactory<ClaimsPrincipal>();
        services.AddScopedFactory<OperationContext>();
    }
}