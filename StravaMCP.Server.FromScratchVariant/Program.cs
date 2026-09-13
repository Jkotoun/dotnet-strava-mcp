using System.Text.Json;
using StravaMCP.Server.FromScratchVariant.JsonRpc;
using StravaMCP.Server.FromScratchVariant.Mcp;
using StravaMCP.Server.FromScratchVariant.Tools;
using StravaMCP.Strava;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStravaApi(builder.Configuration);

// Scoped, not singleton: these depend (transitively) on the typed StravaClient, which AddHttpClient<T>()
// registers as Transient (a new instance per resolution). A singleton registration here would capture
// one StravaClient/HttpClient for the app's entire lifetime, defeating the factory's handler rotation
// (captive dependency) - Scoped keeps a fresh StravaClient per request, same as the tools resolving it.
builder.Services.AddScoped<IMcpTool, GetAthleteProfileTool>();
builder.Services.AddScoped<IMcpTool, GetRecentActivitiesTool>();
builder.Services.AddScoped<IMcpTool, GetActivityDetailTool>();
builder.Services.AddScoped<IMcpTool, GetAthleteStatsTool>();
builder.Services.AddScoped<ToolRegistry>();
builder.Services.AddScoped<McpDispatcher>();

var app = builder.Build();

app.MapPost("/mcp", async (HttpContext context, McpDispatcher dispatcher, CancellationToken ct) =>
{
    JsonRpcRequest request;
    try
    {
        request = await context.Request.ReadFromJsonAsync<JsonRpcRequest>(ct)
            ?? throw new JsonException("Request body was empty.");
    }
    catch (JsonException ex)
    {
        return Results.Ok(new JsonRpcResponse
        {
            Id = null,
            Error = new JsonRpcError { Code = -32700, Message = $"Parse error: {ex.Message}" },
        });
    }

    var response = await dispatcher.HandleAsync(request, ct);

    // A request with no "id" is a JSON-RPC notification; the spec requires no response body.
    return request.Id.ValueKind == JsonValueKind.Undefined
        ? Results.StatusCode(StatusCodes.Status202Accepted)
        : Results.Ok(response);
});

app.Run();

public partial class Program;
