using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace Devocean.Core.Infrastructure.Services.Auth;

public class ClaimsFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ClaimsFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ClaimsPrincipal? GetPrincipal() => _serviceProvider.GetService(typeof(ClaimsPrincipal)) as ClaimsPrincipal;
}