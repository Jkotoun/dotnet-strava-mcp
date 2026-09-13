using System.Text.Json;
using Microsoft.Extensions.Logging;
using StravaMCP.Server.FromScratchVariant.JsonRpc;

namespace StravaMCP.Server.FromScratchVariant.Mcp;

public sealed class McpDispatcher(ToolRegistry registry, ILogger<McpDispatcher> logger)
{
    public async Task<JsonRpcResponse> HandleAsync(JsonRpcRequest request, CancellationToken ct)
    {
        return request.Method switch
        {
            "initialize" => Ok(request.Id, BuildInitializeResult()),
            "tools/list" => Ok(request.Id, BuildToolsListResult()),
            "tools/call" => await HandleToolCallAsync(request, ct),
            _ => Error(request.Id, -32601, $"Method '{request.Method}' not found."),
        };
    }

    private async Task<JsonRpcResponse> HandleToolCallAsync(JsonRpcRequest request, CancellationToken ct)
    {
        if (request.Params is not { } paramsElement || !paramsElement.TryGetProperty("name", out var nameElement))
        {
            return Error(request.Id, -32602, "Missing tool name.");
        }

        var toolName = nameElement.GetString();
        var tool = toolName is not null ? registry.TryGet(toolName) : null;
        if (tool is null)
        {
            return Error(request.Id, -32602, $"Unknown tool: '{toolName}'.");
        }

        var arguments = paramsElement.TryGetProperty("arguments", out var argumentsElement)
            ? argumentsElement
            : default;

        try
        {
            var result = await tool.ExecuteAsync(arguments, ct);
            var text = result is string resultString ? resultString : JsonSerializer.Serialize(result);
            return Ok(request.Id, new { content = new[] { new { type = "text", text } } });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tool '{ToolName}' threw while handling a tools/call request.", toolName);
            return Ok(request.Id, new
            {
                content = new[] { new { type = "text", text = $"An error occurred invoking '{toolName}': {ex.Message}" } },
                isError = true,
            });
        }
    }

    private static object BuildInitializeResult() => new
    {
        protocolVersion = "2025-06-18",
        capabilities = new { tools = new { } },
        serverInfo = new { name = "StravaMCP.Server.FromScratchVariant", version = "1.0.0" },
    };

    private object BuildToolsListResult() => new
    {
        tools = registry.All.Select(tool => new
        {
            name = tool.Name,
            description = tool.Description,
            inputSchema = tool.InputSchema,
        }),
    };

    private static JsonRpcResponse Ok(JsonElement id, object result) => new() { Id = NormalizeId(id), Result = result };

    private static JsonRpcResponse Error(JsonElement id, int code, string message) =>
        new() { Id = NormalizeId(id), Error = new JsonRpcError { Code = code, Message = message } };

    private static JsonElement? NormalizeId(JsonElement id) => id.ValueKind == JsonValueKind.Undefined ? null : id;
}
