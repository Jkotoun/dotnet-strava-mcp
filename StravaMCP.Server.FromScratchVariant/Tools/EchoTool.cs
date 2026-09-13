using System.Text.Json;
using StravaMCP.Server.FromScratchVariant.Mcp;

namespace StravaMCP.Server.FromScratchVariant.Tools;

public sealed class EchoTool : IMcpTool
{
    public string Name => "echo";
    public string Description => "Echoes back the provided message.";

    public JsonElement InputSchema { get; } = JsonDocument.Parse("""
        {"type":"object","properties":{"message":{"type":"string"}},"required":["message"]}
        """).RootElement;

    public Task<object?> ExecuteAsync(JsonElement arguments, CancellationToken ct) =>
        Task.FromResult<object?>(arguments.GetProperty("message").GetString());
}
