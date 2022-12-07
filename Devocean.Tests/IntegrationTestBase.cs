using System.Reflection;
using AutoMapper;
using Devocean.Core.Application.Interfaces;
using Devocean.Core.Application.Mappers.Common;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace Devocean.Tests;

public abstract class IntegrationTestBase<TStartup, TDbContext> : IDisposable
    where TStartup : class
    where TDbContext : DbContext, IDbContext
{
    private readonly bool _shouldEnsureDeleted;
    private readonly bool _shouldEnsureCreated;
    private readonly bool _shouldMigrate;
    static readonly object _locker = new();

    protected CompareLogic CompareLogic { get; set; }
    protected ITestOutputHelper TestOutputHelper { get; set; }
    protected TDbContext DbContext { get; set; }
    protected IMapper Mapper { get; set; }
    protected WebApplicationFactory<TStartup> Factory { get; set; }
    protected IConfiguration Configuration { get; set; }
    protected HttpClient Client { get; set; }

    protected IntegrationTestBase(CompareLogic compareLogic, ITestOutputHelper testOutputHelper, TDbContext dbContext,
        IMapper mapper, WebApplicationFactory<TStartup> factory, HttpClient client)
    {
        CompareLogic = compareLogic;
        TestOutputHelper = testOutputHelper;
        DbContext = dbContext;
        Mapper = mapper;
        Factory = factory;
        Client = client;
    }

    protected IntegrationTestBase(ITestOutputHelper testOutputHelper,
        bool shouldEnsureDeleted = false, bool shouldEnsureCreated = false,
        bool shouldMigrate = false,
        Action<TDbContext>? seedFunc = null,
        Assembly? automapperProfileAssembly = null)
    {
        _shouldEnsureDeleted = shouldEnsureDeleted;
        _shouldEnsureCreated = shouldEnsureCreated;
        _shouldMigrate = shouldMigrate;
        TestOutputHelper = testOutputHelper;
        CompareLogic = new CompareLogic();

        if (automapperProfileAssembly != null) AutomapperProfile.IncludeAssembly(automapperProfileAssembly);

        SetFactory(new WebApplicationFactory<TStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(
                    serviceCollection => ConfigureInjectedServices(serviceCollection, builder));
            }));

        lock (_locker)
        {
            if (_shouldEnsureDeleted) DbContext.Database.EnsureDeleted();
            if (_shouldMigrate) DbContext.Database.Migrate();
            if (_shouldEnsureCreated) DbContext.Database.EnsureCreated();
            seedFunc?.Invoke(DbContext);
        }
    }

    protected void SetFactory(WebApplicationFactory<TStartup> webApplicationFactory)
    {
        Factory = webApplicationFactory;
        Client = Factory.CreateClient();
        var serviceScope = Factory.Services.CreateScope();
        Configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();
        SetLogLevel(LogEventLevel.Information);
        Mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
        DbContext = serviceScope
                        .ServiceProvider
                        .GetRequiredService(typeof(TDbContext)) as TDbContext
                    ?? throw new NullReferenceException("[DbContext] cannot be null");
    }
    
    protected void SetLogLevel(LogEventLevel logLevel)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.XunitTestOutput(TestOutputHelper)
            .CreateLogger();
    }

    protected virtual void ConfigureInjectedServices(IServiceCollection serviceCollection,
        IWebHostBuilder webHostBuilder)
    {
    }

    public void Dispose()
    {
        DbContext.Dispose();
        Factory.Dispose();
        Client.Dispose();
    }
}

public abstract class IntegrationTestBase<TStartup> : IDisposable
    where TStartup : class
{
    protected CompareLogic CompareLogic { get; set; }
    protected ITestOutputHelper TestOutputHelper { get; set; }
    protected IMapper Mapper { get; set; }
    protected IConfiguration Configuration { get; set; }
    protected WebApplicationFactory<TStartup> Factory { get; set; }
    protected HttpClient Client { get; set; }

    protected IntegrationTestBase(CompareLogic compareLogic, ITestOutputHelper testOutputHelper,
        IMapper mapper, WebApplicationFactory<TStartup> factory, HttpClient client)
    {
        CompareLogic = compareLogic;
        TestOutputHelper = testOutputHelper;
        Mapper = mapper;
        Factory = factory;
        Client = client;
    }

    protected IntegrationTestBase(ITestOutputHelper testOutputHelper,
        Assembly? automapperProfileAssembly = null)
    {
        TestOutputHelper = testOutputHelper;
        CompareLogic = new CompareLogic();

        if (automapperProfileAssembly != null) AutomapperProfile.IncludeAssembly(automapperProfileAssembly);

        SetFactory(new WebApplicationFactory<TStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(
                    serviceCollection => ConfigureInjectedServices(serviceCollection, builder));
            }));
    }

    protected void SetFactory(WebApplicationFactory<TStartup> webApplicationFactory)
    {
        Factory = webApplicationFactory;
        Client = Factory.CreateClient();
        var serviceScope = Factory.Services.CreateScope();
        Mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
        Configuration = serviceScope.ServiceProvider.GetService<IConfiguration>();
        SetLogLevel(LogEventLevel.Information);
    }
    
    protected void SetLogLevel(LogEventLevel logLevel)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.XunitTestOutput(TestOutputHelper)
            .CreateLogger();
    }

    protected virtual void ConfigureInjectedServices(IServiceCollection serviceCollection,
        IWebHostBuilder webHostBuilder)
    {
    }

    public void Dispose()
    {
        Factory.Dispose();
        Client.Dispose();
    }
}