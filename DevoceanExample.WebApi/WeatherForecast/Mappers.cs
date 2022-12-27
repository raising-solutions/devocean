using AutoMapper;
using Devocean.Core.Application.Mappers.Common;
using DevoceanExample.WebApi.WeatherForecast.Apis.Models;
using DevoceanExample.WebApi.WeatherForecast.UseCases;

namespace DevoceanExample.WebApi.WeatherForecast;

public class Mappers : IAutomapperProfile //IAutomapFrom<WeatherForecastModel>
{
    public void Setup(Profile profile)
    {
        profile.CreateMap<WeatherForecastModel, CreateWeatherForecast>();
        profile.CreateMap<WeatherForecastEntity, WeatherForecastViewModel>();

        profile.CreateMap<CreateWeatherForecast, WeatherForecastEntity>()
        .ForMember(entity => entity.Created,
            cfg => cfg.Ignore())
        .ForMember(entity => entity.CreatedBy,
            cfg => cfg.Ignore())
        .ForMember(entity => entity.LastModified,
            cfg => cfg.Ignore())
        .ForMember(entity => entity.LastModifiedBy,
            cfg => cfg.Ignore())
        .ForMember(entity => entity.Id,
            cfg => cfg.Ignore());
    }
}
