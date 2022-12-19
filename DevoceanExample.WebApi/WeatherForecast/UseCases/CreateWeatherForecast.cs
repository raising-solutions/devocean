using AutoMapper;
using Devocean.Core.Application.UseCases.Common;
using Devocean.Core.Extensions;
using DevoceanExample.WebApi.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using System.ComponentModel;

namespace DevoceanExample.WebApi.WeatherForecast.UseCases;

public class CreateWeatherForecast : IRequest<WeatherForecastEntity>
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    public string? Summary { get; set; }

    public CreateWeatherForecast()
    {

    }

    public CreateWeatherForecast(DateOnly date, int temperatureC, int temperatureF, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        TemperatureF = temperatureF;
        Summary = summary;
    }

    public class Validator : AbstractValidator<CreateWeatherForecast>
    {
        public Validator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(v => v.Date)
                .NotEmpty()
                .WithMessageEnum(Error.DateRequired);

            RuleFor(v => v.Summary)
                .NotEmpty()
                .WithMessageEnum(Error.SummaryRequired);

            RuleFor(v => v.TemperatureC)
                .NotEmpty()
                .WithMessageEnum(Error.TemperatureCRequired);
        }
    }

    public enum Error
    {
        [Description("O destino é obrigatório")]
        DestinationRequired,
        [Description("A Data é obrigatória")] DateRequired,
        [Description("O Sumario é obrigatório")] SummaryRequired,
        [Description("A Temperatura é obrigatório")] TemperatureCRequired
    }

    public class Handler : HandlerBase<CreateWeatherForecast, WeatherForecastEntity,
        ApplicationDbContext>
    {
        private readonly ISender _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Handler> _logger;

        public Handler(ApplicationDbContext context, IMapper mapper,
            ISender mediator, IConfiguration configuration,
            ILogger<Handler> logger) : base(context, mapper)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        public override async Task<WeatherForecastEntity?> Handle(CreateWeatherForecast request,
            CancellationToken cancellationToken)
        {
            var weatherForecast = _mapper.Map<WeatherForecastEntity>(request);

            var entry = _dbContext.WeatherForecast.Add(weatherForecast);
            weatherForecast = entry.Entity;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return weatherForecast;
        }
    }
}