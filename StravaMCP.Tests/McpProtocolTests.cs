using System.Text.Json;
using StravaMCP.Tests.Common;
using Xunit;

namespace StravaMCP.Tests;

public sealed class McpProtocolTests : IClassFixture<StravaFakeWebApplicationFactory<Program>>
{
    private readonly McpTestClient _client;

    public McpProtocolTests(StravaFakeWebApplicationFactory<Program> factory)
    {
        _client = new McpTestClient(factory.CreateClient());
    }

    [Fact]
    public async Task Initialize_ReturnsCapabilities()
    {
        var response = await _client.SendAsync("initialize", new
        {
            protocolVersion = "2025-06-18",
            capabilities = new { },
            clientInfo = new { name = "test-client", version = "0.1" },
        });

        Assert.Null(response.Error);
        Assert.NotNull(response.Result);
        Assert.True(response.Result!.Value.TryGetProperty("protocolVersion", out _));
        Assert.True(response.Result!.Value.TryGetProperty("capabilities", out _));
    }

    [Fact]
    public async Task ToolsList_IncludesExpectedTools()
    {
        var response = await _client.SendAsync("tools/list");

        Assert.Null(response.Error);
        Assert.NotNull(response.Result);

        var toolNames = response.Result!.Value
            .GetProperty("tools")
            .EnumerateArray()
            .Select(tool => tool.GetProperty("name").GetString())
            .ToList();

        Assert.Contains("get_athlete_profile", toolNames);
        Assert.Contains("get_recent_activities", toolNames);
        Assert.Contains("get_activity_detail", toolNames);
        Assert.Contains("get_athlete_stats", toolNames);
    }

    [Fact]
    public async Task ToolsCall_GetAthleteProfile_ReturnsExpectedResult()
    {
        var response = await _client.SendAsync("tools/call", new
        {
            name = "get_athlete_profile",
            arguments = new { },
        });

        Assert.Null(response.Error);
        using var content = JsonDocument.Parse(GetFirstContentText(response.Result!.Value)!);
        Assert.Equal(1, content.RootElement.GetProperty("id").GetInt32());
        Assert.Equal("fake_athlete", content.RootElement.GetProperty("username").GetString());
    }

    [Fact]
    public async Task ToolsCall_GetRecentActivities_ReturnsExpectedResult()
    {
        var response = await _client.SendAsync("tools/call", new
        {
            name = "get_recent_activities",
            arguments = new { count = 5 },
        });

        Assert.Null(response.Error);
        using var content = JsonDocument.Parse(GetFirstContentText(response.Result!.Value)!);
        var activity = Assert.Single(content.RootElement.EnumerateArray());
        Assert.Equal("Fake Ride", activity.GetProperty("name").GetString());
    }

    [Fact]
    public async Task ToolsCall_GetActivityDetail_ReturnsExpectedResult()
    {
        var response = await _client.SendAsync("tools/call", new
        {
            name = "get_activity_detail",
            arguments = new { activityId = 100 },
        });

        Assert.Null(response.Error);
        using var content = JsonDocument.Parse(GetFirstContentText(response.Result!.Value)!);
        Assert.Equal("A fake ride for tests", content.RootElement.GetProperty("description").GetString());
    }

    [Fact]
    public async Task ToolsCall_GetAthleteStats_ReturnsExpectedResult()
    {
        var response = await _client.SendAsync("tools/call", new
        {
            name = "get_athlete_stats",
            arguments = new { },
        });

        Assert.Null(response.Error);
        using var content = JsonDocument.Parse(GetFirstContentText(response.Result!.Value)!);
        Assert.Equal(50000, content.RootElement.GetProperty("biggest_ride_distance").GetInt32());
        Assert.Equal(20, content.RootElement.GetProperty("all_run_totals").GetProperty("count").GetInt32());
    }

    [Fact]
    public async Task ToolsCall_UnknownTool_ReturnsJsonRpcError()
    {
        var response = await _client.SendAsync("tools/call", new
        {
            name = "not_a_real_tool",
            arguments = new { },
        });

        Assert.NotNull(response.Error);
        Assert.Equal(-32602, response.Error!.Value.GetProperty("code").GetInt32());
    }

    [Fact]
    public async Task UnknownMethod_ReturnsMethodNotFoundError()
    {
        var response = await _client.SendAsync("totally/bogus");

        Assert.NotNull(response.Error);
        Assert.Equal(-32601, response.Error!.Value.GetProperty("code").GetInt32());
    }

    private static string? GetFirstContentText(JsonElement result) =>
        result.GetProperty("content")[0].GetProperty("text").GetString();
}
