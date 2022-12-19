using Devocean.Core.Application.Interfaces;
using Devocean.Core.Application.Mappers.Common;
using Devocean.Core.Application.UseCases.Common;
using Devocean.Core.Entrypoints.Web.Common;
using Devocean.Core.Infrastructure.Services;
using Devocean.Core.Infrastructure.Services.Mapping;
using DevoceanExample.WebApi.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Reflection;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var executingAssembly = Assembly.GetExecutingAssembly();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["AppSettings:DB_CONN"];
var smtpConfigEndpoint = builder.Configuration["AppSettings:SmtpConfig:Endpoint"];
int smtpConfigPort = Convert.ToInt32(builder.Configuration["AppSettings:SmtpConfig:Port"]);
var smtpConfigUsername = builder.Configuration["AppSettings:SmtpConfig:Username"];
var smtpConfigPassword = builder.Configuration["AppSettings:SmtpConfig:Password"];

var awsAccessKey = builder.Configuration["AppSettings:Aws:AccessKey"];
var awsSecretKey = builder.Configuration["AppSettings:Aws:SecretKey"];

 

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString,
        optionsBuilder =>
        {
            optionsBuilder.MigrationsHistoryTable("__EFMigrationsHistory", ApplicationDbContext.Schema);
        }));

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => { options.UseInlineDefinitionsForEnums(); });
builder.Services.AddSwaggerGenNewtonsoftSupport();

// Custom Injections
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<IDateTime, DateTimeService>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

builder.Services.AddAutoMapper(AutomapperFactory.ConfigureScan<Program>());
builder.Services.AddHostedService<AutomapperConfigurationBackgroundService>();

builder.Services.AddValidatorsFromAssembly(executingAssembly);
builder.Services.AddMediatR(new[] { executingAssembly },
    cfg => cfg.AsScoped());

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WeatherForecastService",
        Description = "V1 WeatherForecast Services"
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options => { options.SerializeAsV2 = true; });
    app.UseSwaggerUI();
    app.UseCors(policyBuilder => policyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
}
app.UseSerilogRequestLogging();

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();

namespace DevoceanExample.WebApi
{
    public partial class Program
    {
    }
}