using System.Net;
using System.Net.Http.Json;
using CryptoAlarmSystem.Application.DTOs;
using CryptoAlarmSystem.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CryptoAlarmSystem.IntegrationTests.Controllers;

public class AlarmsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private const string BaseUrl = "/api/v1/alarms";
    private const string TestUserId = "test-user-123";

    public AlarmsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.SeedDatabase(); // Seed database before each test class
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-User-Id", TestUserId);
    }

    [Fact]
    public async Task GetCryptoSymbols_ShouldReturnListOfSymbols()
    {
        var response = await _client.GetAsync($"{BaseUrl}/crypto-symbols");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var symbols = await response.Content.ReadFromJsonAsync<List<CryptoSymbolResponse>>();
        symbols.Should().NotBeNull();
        symbols.Should().HaveCountGreaterThan(0);
        symbols.Should().Contain(s => s.Code == "BTC");
    }

    [Fact]
    public async Task GetNotificationChannels_ShouldReturnListOfChannels()
    {
        var response = await _client.GetAsync($"{BaseUrl}/notification-channels");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var channels = await response.Content.ReadFromJsonAsync<List<NotificationChannelDto>>();
        channels.Should().NotBeNull();
        channels.Should().HaveCount(3);
        channels.Should().Contain(c => c.Code == "EMAIL");
    }

    [Fact]
    public async Task GetAlarmTypes_ShouldReturnListOfTypes()
    {
        var response = await _client.GetAsync($"{BaseUrl}/alarm-types");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var types = await response.Content.ReadFromJsonAsync<List<AlarmTypeResponse>>();
        types.Should().NotBeNull();
        types.Should().HaveCount(2);
        types.Should().Contain(t => t.Code == "ABOVE");
        types.Should().Contain(t => t.Code == "BELOW");
    }

    [Fact]
    public async Task CreateAlarm_WithValidData_ShouldReturnCreated()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var response = await _client.PostAsJsonAsync(BaseUrl, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var alarm = await response.Content.ReadFromJsonAsync<AlarmResponse>();
        alarm.Should().NotBeNull();
        alarm!.UserId.Should().Be(TestUserId);
        alarm.TargetPrice.Should().Be(45000m);
        alarm.CryptoSymbolCode.Should().Be("BTC");
    }

    [Fact]
    public async Task CreateAlarm_WithInvalidSymbolId_ShouldReturnBadRequest()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 999,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var response = await _client.PostAsJsonAsync(BaseUrl, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAlarm_WithNegativePrice_ShouldReturnBadRequest()
    {
        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: -100m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var response = await _client.PostAsJsonAsync(BaseUrl, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAlarm_WithoutUserId_ShouldReturnBadRequest()
    {
        var client = _client;
        client.DefaultRequestHeaders.Remove("X-User-Id");

        var request = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 45000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );

        var response = await client.PostAsJsonAsync(BaseUrl, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        client.DefaultRequestHeaders.Add("X-User-Id", TestUserId);
    }

    [Fact]
    public async Task GetActiveAlarms_ShouldReturnPagedResponse()
    {
        var createRequest = new CreateAlarmRequest(
            CryptoSymbolId: 2,
            AlarmTypeId: AlarmTypes.Below,
            TargetPrice: 2000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Sms }
        );
        await _client.PostAsJsonAsync(BaseUrl, createRequest);

        var response = await _client.GetAsync($"{BaseUrl}/active?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
        content.Should().Contain("pageNumber");
        content.Should().Contain("totalRecords");
    }

    [Fact]
    public async Task DeleteAlarm_ExistingAlarm_ShouldReturnNoContent()
    {
        var createRequest = new CreateAlarmRequest(
            CryptoSymbolId: 3,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 100m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.PushNotification }
        );
        var createResponse = await _client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdAlarm = await createResponse.Content.ReadFromJsonAsync<AlarmResponse>();

        var response = await _client.DeleteAsync($"{BaseUrl}/{createdAlarm!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteAlarm_NonExistingAlarm_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync($"{BaseUrl}/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAlarmChannels_ValidRequest_ShouldReturnOk()
    {
        var createRequest = new CreateAlarmRequest(
            CryptoSymbolId: 1,
            AlarmTypeId: AlarmTypes.Above,
            TargetPrice: 46000m,
            NotificationChannelIds: new List<NotificationChannels> { NotificationChannels.Email }
        );
        var createResponse = await _client.PostAsJsonAsync(BaseUrl, createRequest);
        var createdAlarm = await createResponse.Content.ReadFromJsonAsync<AlarmResponse>();

        var updateRequest = new UpdateAlarmChannelsRequest(
            NotificationChannelIds: new List<NotificationChannels>
            {
                NotificationChannels.Email,
                NotificationChannels.Sms
            }
        );

        var response = await _client.PatchAsJsonAsync($"{BaseUrl}/{createdAlarm!.Id}/channels", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedAlarm = await response.Content.ReadFromJsonAsync<AlarmResponse>();
        updatedAlarm!.NotificationChannels.Should().HaveCount(2);
    }
}
