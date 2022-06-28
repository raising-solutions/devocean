using System.Reflection;
using AutoMapper;
using Devocean.Core.Application.Interfaces;
using Devocean.Core.Application.Mappers.Common;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
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

    protected CompareLogic _compareLogic;
    protected ITestOutputHelper TestOutputHelper { get; set; }
    protected TDbContext DbContext { get; set; }
    protected IMapper Mapper { get; set; }
    protected WebApplicationFactory<TStartup> Factory { get; set; }
    protected HttpClient Client { get; set; }

    protected IntegrationTestBase(CompareLogic compareLogic, ITestOutputHelper testOutputHelper, TDbContext dbContext,
        IMapper mapper, WebApplicationFactory<TStartup> factory, HttpClient client)
    {
        _compareLogic = compareLogic;
        TestOutputHelper = testOutputHelper;
        DbContext = dbContext;
        Mapper = mapper;
        Factory = factory;
        Client = client;
    }

    protected IntegrationTestBase(ITestOutputHelper testOutputHelper,
        Func<IWebHostBuilder, Action<DbContextOptionsBuilder>> dbContextOptionsBuilder,
        bool shouldEnsureDeleted = false, bool shouldEnsureCreated = false,
        bool shouldMigrate = false,
        Action<TDbContext>? seedFunc = null,
        Assembly? automapperProfileAssembly = null)
    {
        _shouldEnsureDeleted = shouldEnsureDeleted;
        _shouldEnsureCreated = shouldEnsureCreated;
        _shouldMigrate = shouldMigrate;
        TestOutputHelper = testOutputHelper;
        _compareLogic = new CompareLogic();
        
        AutomapperProfile.IncludedAssemblies.Add(typeof(Program).Assembly);
        if (automapperProfileAssembly != null) AutomapperProfile.IncludedAssemblies.Add(automapperProfileAssembly);

        Factory = new WebApplicationFactory<TStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAutoMapper(typeof(AutomapperProfile).Assembly);
                    services.AddDbContext<TDbContext>(dbContextOptionsBuilder(builder));
                });
                // ... Configure test services
            });

        Client = Factory.CreateClient();
        var serviceScope = Factory.Services.CreateScope();
        Mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
        DbContext = serviceScope
                        .ServiceProvider
                        .GetRequiredService(typeof(TDbContext)) as TDbContext
                    ?? throw new NullReferenceException("[DbContext] cannot be null");

        lock (_locker)
        {
            if (_shouldEnsureDeleted) DbContext.Database.EnsureDeleted();
            if (_shouldEnsureCreated) DbContext.Database.EnsureCreated();
            if (_shouldMigrate) DbContext.Database.Migrate();
            seedFunc?.Invoke(DbContext);
        }
    }

    public void Dispose()
    {
        DbContext.Dispose();
        Factory.Dispose();
        Client.Dispose();
    }
}