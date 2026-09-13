using System.Text.Json;
using StravaMCP.Server.FromScratchVariant.Mcp;

namespace StravaMCP.Server.FromScratchVariant.Tools;

public sealed class AddNumbersTool : IMcpTool
{
    public string Name => "add_numbers";
    public string Description => "Adds two numbers together.";

    public JsonElement InputSchema { get; } = JsonDocument.Parse("""
        {"type":"object","properties":{"a":{"type":"integer"},"b":{"type":"integer"}},"required":["a","b"]}
        """).RootElement;

    public Task<object?> ExecuteAsync(JsonElement arguments, CancellationToken ct)
    {
        var a = arguments.GetProperty("a").GetInt32();
        var b = arguments.GetProperty("b").GetInt32();
        return Task.FromResult<object?>(a + b);
    }
}
