using Devocean.Core.Extensions;
using DevoceanExample.WebApi.Tests.FakeData;
using DevoceanExample.WebApi.WeatherForecast.Apis.Models;
using DevoceanExample.WebApi.WeatherForecast.UseCases;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace DevoceanExample.WebApi.Tests.IntegrationTests;

public class WeatherForecastIntegrationTests : IntegrationTestsSetup
{
    string _pathWeatherForecastApiV1 = "/api/forecast/v1";

    public WeatherForecastIntegrationTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
        Configuration["AppSettings:Feat_SendNotificationAfterCreate"] = "false";
    }

    [Fact]
    public async void GivenDoGetForAllNotificationControls_ReturnsOkWithAllWeatherForecasts()
    {
        // Given
        var totalNotifications = DbContext.WeatherForecast.Count();

        // When
        var response = await Client.GetAsync(_pathWeatherForecastApiV1);
        var responseData = JsonConvert
            .DeserializeObject<List<WeatherForecastViewModel>>(await response.Content.ReadAsStringAsync());

        // Then
        responseData.Should().NotBeNull();
        responseData?.Count.Should().Be(totalNotifications);
    }

    [Fact]
    public async void GivenDoGetWithIdForOneNotificationControl_ReturnsOkWithOneWeatherForecast()
    {
        // Given
        var randomNotificationControl = await FindRandomWeatherForecast();
        var expectedReturn = Mapper.Map<WeatherForecastViewModel>(randomNotificationControl);

        // When
        var response = await Client.GetAsync($"{_pathWeatherForecastApiV1}/{randomNotificationControl.Id}");
        var responseData = JsonConvert
            .DeserializeObject<WeatherForecastViewModel>(await response.Content.ReadAsStringAsync());

        // Then
        var comparisonResult = CompareLogic.Compare(expectedReturn, responseData);
        TestOutputHelper.WriteLine(comparisonResult.DifferencesString);

        responseData.Should().NotBeNull();
        comparisonResult.AreEqual.Should().Be(true);
    }

    [Fact]
    public async void GivenDoPostWithOneWeatherForecast_ReturnsCreatedWithLocalization()
    {
        //Given
        var payload = WeatherForecastFaker.GetWeatherForecast();

        // when
        var response = await Client.PostAsJsonAsync(_pathWeatherForecastApiV1, payload);

        //then
        var checkedNotificationControl = await DbContext.WeatherForecast
            .FirstOrDefaultAsync(forecast => forecast.Date == payload.Date &&
                                             forecast.Summary == payload.Summary);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        checkedNotificationControl.Should().NotBeNull();
        response.Headers.Location!.PathAndQuery.Should()
            .Be($"{_pathWeatherForecastApiV1}/{checkedNotificationControl?.Id}");
    }

    [Fact]
    public async void GivenDoPostWithEmptyWeatherForecast_ReturnsUnprocessableEntityWithErrors()
    {
        // when
        var response = await Client.PostAsJsonAsync(_pathWeatherForecastApiV1,
            new WeatherForecastModel() { });
        var result = await response.Content.ReadAsStringAsync();

        //then
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        result.Should().Contain(CreateWeatherForecast.Error.DestinationRequired.GetDescription());
    }
}
