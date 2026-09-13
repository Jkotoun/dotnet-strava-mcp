using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StravaMCP.Tests;

public sealed record JsonRpcTestResponse(JsonElement? Result, JsonElement? Error);

public sealed class McpTestClient(HttpClient httpClient)
{
    public async Task<JsonRpcTestResponse> SendAsync(
        string method,
        object? @params = null,
        int id = 1,
        CancellationToken ct = default)
    {
        var requestBody = new JsonRpcRequestDto
        {
            Id = id,
            Method = method,
            Params = @params,
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = JsonContent.Create(requestBody),
        };
        httpRequest.Headers.Accept.ParseAdd("application/json");
        httpRequest.Headers.Accept.ParseAdd("text/event-stream");

        using var httpResponse = await httpClient.SendAsync(httpRequest, ct);
        httpResponse.EnsureSuccessStatusCode();

        var body = await httpResponse.Content.ReadAsStringAsync(ct);
        var json = ExtractJsonPayload(body);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var result = root.TryGetProperty("result", out var resultElement) ? resultElement.Clone() : (JsonElement?)null;
        var error = root.TryGetProperty("error", out var errorElement) ? errorElement.Clone() : (JsonElement?)null;

        return new JsonRpcTestResponse(result, error);
    }

    private static string ExtractJsonPayload(string body)
    {
        var trimmed = body.TrimStart();
        if (!trimmed.StartsWith("event:", StringComparison.Ordinal) &&
            !trimmed.StartsWith("data:", StringComparison.Ordinal))
        {
            // Not SSE-framed - the whole body is already the JSON payload.
            return body;
        }

        var dataLines = body
            .Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .Where(line => line.StartsWith("data:", StringComparison.Ordinal))
            .Select(line => line["data:".Length..].TrimStart());

        return string.Join('\n', dataLines);
    }

    private sealed class JsonRpcRequestDto
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; init; } = "2.0";

        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("method")]
        public required string Method { get; init; }

        [JsonPropertyName("params")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Params { get; init; }
    }
}
