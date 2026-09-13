using StravaMCP.Server.FromScratchVariant.JsonRpc;
using StravaMCP.Server.FromScratchVariant.Mcp;
using StravaMCP.Server.FromScratchVariant.Tools;
using StravaMCP.Strava;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StravaOptions>(builder.Configuration.GetSection("Strava"));
builder.Services.AddHttpClient("StravaAuth");
builder.Services.AddSingleton<StravaAuthClient>();
builder.Services.AddTransient<StravaAuthHandler>();
builder.Services
    .AddHttpClient<StravaClient>()
    .AddHttpMessageHandler<StravaAuthHandler>();

builder.Services.AddSingleton<IMcpTool, EchoTool>();
builder.Services.AddSingleton<IMcpTool, AddNumbersTool>();
builder.Services.AddSingleton<IMcpTool, GetAthleteProfileTool>();
builder.Services.AddSingleton<IMcpTool, GetRecentActivitiesTool>();
builder.Services.AddSingleton<IMcpTool, GetActivityDetailTool>();
builder.Services.AddSingleton<IMcpTool, GetAthleteStatsTool>();
builder.Services.AddSingleton<ToolRegistry>();
builder.Services.AddSingleton<McpDispatcher>();

var app = builder.Build();

app.MapPost("/mcp", async (JsonRpcRequest request, McpDispatcher dispatcher, CancellationToken ct) =>
    Results.Ok(await dispatcher.HandleAsync(request, ct)));

app.Run();

public partial class Program;
