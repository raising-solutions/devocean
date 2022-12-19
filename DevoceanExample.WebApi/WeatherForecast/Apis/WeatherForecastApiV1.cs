using AutoMapper;
using Devocean.Core.Entrypoints.Web.Common;
using DevoceanExample.WebApi.Infrastructure.Persistence;
using DevoceanExample.WebApi.WeatherForecast.Apis.Models;
using DevoceanExample.WebApi.WeatherForecast.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace DevoceanExample.WebApi.WeatherForecast.Apis;

[ApiController]
[Route("api/forecast/v1")]
public class WeatherForecastApiV1 : ApiControlerBase
{
    private readonly ILogger<WeatherForecastApiV1> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public WeatherForecastApiV1(ILogger<WeatherForecastApiV1> logger,
        ApplicationDbContext context, IMapper mapper)
    {
        _logger = logger;
        _dbContext = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async ValueTask<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var integrations = await Mediator.Send(new GetAllWeatherForecasts(),
            cancellationToken).ConfigureAwait(false);

        var response = _mapper.Map<List<WeatherForecastViewModel>>(integrations);
        return integrations.Any() ? Ok(response) : NoContent();
    }

    [HttpGet]
    [Route("{id}")]
    public async ValueTask<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var integrationControl = await _dbContext.WeatherForecast.FindAsync(id);
        var retorno = _mapper.Map<WeatherForecastViewModel>(integrationControl);
        return Ok(retorno);
    }

    [HttpPost]
    public async ValueTask<IActionResult> Post(WeatherForecastModel payload, CancellationToken cancellationToken)
    {
        var createNotification = _mapper.Map<CreateWeatherForecast>(payload);

        var response = await Mediator.Send(createNotification, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = response.Id }, null);
    }
}