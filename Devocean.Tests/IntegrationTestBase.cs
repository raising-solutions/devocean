using AutoMapper;
using Devocean.Core.Application.Interfaces;
using KellermanSoftware.CompareNetObjects;
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
    private readonly bool _shouldMidrate;
    static readonly object _locker = new();

    protected CompareLogic _compareLogic;
    protected ITestOutputHelper TestOutputHelper { get; set; }
    protected TDbContext DbContext { get; set; }
    protected IMapper Mapper { get; set; }
    protected WebApplicationFactory<TStartup> Factory { get; set; }
    protected HttpClient Client { get; set; }

    protected IntegrationTestBase(ITestOutputHelper testOutputHelper, Action<DbContextOptionsBuilder>? optionsAction,
        Action<TDbContext> seedFunc, bool shouldEnsureDeleted = false, bool shouldEnsureCreated = false, bool shouldMidrate = false)
    {
        _shouldEnsureDeleted = shouldEnsureDeleted;
        _shouldEnsureCreated = shouldEnsureCreated;
        _shouldMidrate = shouldMidrate;
        TestOutputHelper = testOutputHelper;
        _compareLogic = new CompareLogic();

        Factory = new WebApplicationFactory<TStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAutoMapper(typeof(TStartup)); 
                    services.AddDbContext<TDbContext>(optionsAction);
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
            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();
            DbContext.Database.Migrate();
            seedFunc(DbContext);
        }
    }

    public void Dispose()
    {
        DbContext.Dispose();
        Factory.Dispose();
        Client.Dispose();
    }
}