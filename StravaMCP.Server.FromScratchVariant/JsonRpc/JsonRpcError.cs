using System.Text.Json.Serialization;

namespace StravaMCP.Server.FromScratchVariant.JsonRpc;

public sealed class JsonRpcError
{
    [JsonPropertyName("code")]
    public required int Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; init; }
}
