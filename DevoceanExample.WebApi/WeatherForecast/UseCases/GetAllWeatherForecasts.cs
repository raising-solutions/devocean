using AutoMapper;
using Devocean.Core.Application.UseCases.Common;
using DevoceanExample.WebApi.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevoceanExample.WebApi.WeatherForecast.UseCases;

public class GetAllWeatherForecasts : IRequest<List<WeatherForecastEntity>>
{
    public class Handler : HandlerBase<GetAllWeatherForecasts, List<WeatherForecastEntity>, ApplicationDbContext>
    {
        public Handler(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<List<WeatherForecastEntity>> Handle(GetAllWeatherForecasts request, CancellationToken cancellationToken)
        {
            var integrations = await _dbContext.WeatherForecast
                .ToListAsync(cancellationToken: cancellationToken);

            return integrations;
        }
    }
}