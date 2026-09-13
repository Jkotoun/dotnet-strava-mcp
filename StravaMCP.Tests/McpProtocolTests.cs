using Microsoft.AspNetCore.Mvc.Testing;
using StravaMCP.Tests.Common;
using Xunit;

namespace StravaMCP.Tests;

public sealed class McpProtocolTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly McpTestClient _client;

    public McpProtocolTests(WebApplicationFactory<Program> factory)
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
}
