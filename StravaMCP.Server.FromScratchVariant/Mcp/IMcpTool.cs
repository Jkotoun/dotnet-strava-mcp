using System.Text.Json;

namespace StravaMCP.Server.FromScratchVariant.Mcp;

public interface IMcpTool
{
    string Name { get; }
    string Description { get; }
    JsonElement InputSchema { get; }
    Task<object?> ExecuteAsync(JsonElement arguments, CancellationToken ct);
}
