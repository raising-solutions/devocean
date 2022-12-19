using DevoceanExample.WebApi.Tests.FakeData;
using DevoceanExample.WebApi.Tests.Util;
using DevoceanExample.WebApi.WeatherForecast.UseCases;

namespace DevoceanExample.WebApi.Tests.UnitTests.UseCase
{
    public class CreateWeatherForecastTests
    {
        readonly ValidatorUtil<CreateWeatherForecast.Validator, CreateWeatherForecast> _validatorUtil;

        public CreateWeatherForecastTests()
        {
            _validatorUtil = new ValidatorUtil<CreateWeatherForecast.Validator,
                CreateWeatherForecast>(new CreateWeatherForecast.Validator());
        }

        private static CreateWeatherForecast CreateBasicDada()
        {
            return WeatherForecastFaker.GetFakerCreateWeatherForecast();
        }
    }
}
