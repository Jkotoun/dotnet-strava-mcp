using System.Text.Json;
using System.Text.Json.Serialization;

namespace StravaMCP.Server.FromScratchVariant.JsonRpc;

public sealed class JsonRpcResponse
{
    [JsonPropertyName("jsonrpc")]
    public string Jsonrpc { get; init; } = "2.0";

    // Nullable, not a bare JsonElement: the id is JSON `null` when it couldn't be determined (parse
    // errors) or when it never existed (notifications). A default(JsonElement) has ValueKind.Undefined,
    // which System.Text.Json cannot serialize at all - it throws instead of writing anything.
    [JsonPropertyName("id")]
    public JsonElement? Id { get; init; }

    [JsonPropertyName("result")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Result { get; init; }

    [JsonPropertyName("error")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonRpcError? Error { get; init; }
}
