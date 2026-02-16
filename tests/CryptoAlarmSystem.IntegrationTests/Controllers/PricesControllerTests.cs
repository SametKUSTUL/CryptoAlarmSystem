using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace CryptoAlarmSystem.IntegrationTests.Controllers;

public class PricesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private const string BaseUrl = "/api/v1/prices";

    public PricesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdatePrice_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: 1, Price: 45000m);

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/update", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePrice_WithInvalidSymbolId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: 0, Price: 45000m);

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/update", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePrice_WithNegativePrice_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: 1, Price: -100m);

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/update", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePrice_WithZeroPrice_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: 1, Price: 0m);

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/update", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(1, 45000)]
    [InlineData(2, 2500)]
    [InlineData(3, 100)]
    public async Task UpdatePrice_WithDifferentSymbols_ShouldReturnOk(int symbolId, decimal price)
    {
        // Arrange
        var request = new PriceUpdateRequest(CryptoSymbolId: symbolId, Price: price);

        // Act
        var response = await _client.PostAsJsonAsync($"{BaseUrl}/update", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

public record PriceUpdateRequest(int CryptoSymbolId, decimal Price);
