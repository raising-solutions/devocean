using Microsoft.Extensions.DependencyInjection;

namespace Devocean.Core.Infrastructure.Services;

public static partial class ServiceCollectionExtensions
{
    public static void AddTransientFactory<TService>(this IServiceCollection services) 
        where TService : class
    {
        services.AddTransient<TService>();
        services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>()!);
        services.AddSingleton<Factory<TService>>();
    }
    
    public static void AddTransientFactory<TService, TImplementation>(this IServiceCollection services) 
        where TService : class
        where TImplementation : class, TService
    {
        services.AddTransient<TService, TImplementation>();
        services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>()!);
        services.AddSingleton<Factory<TService>>();
    }
    
    public static void AddScopedFactory<TService>(this IServiceCollection services) 
        where TService : class
    {
        services.AddScoped<TService>();
        services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>()!);
        services.AddSingleton<Factory<TService>>();
    }
    
    public static void AddScopedFactory<TService, TImplementation>(this IServiceCollection services) 
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TService, TImplementation>();
        services.AddSingleton<Func<TService>>(x => () => x.GetService<TService>()!);
        services.AddSingleton<Factory<TService>>();
    }
}