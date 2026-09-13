using StravaMCP.Strava;
using StravaMCP.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<StravaOptions>(builder.Configuration.GetSection("Strava"));
builder.Services.AddHttpClient("StravaAuth");
builder.Services.AddSingleton<StravaAuthClient>();
builder.Services.AddTransient<StravaAuthHandler>();
builder.Services
    .AddHttpClient<StravaClient>()
    .AddHttpMessageHandler<StravaAuthHandler>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapMcp("/mcp");

app.Run();

public partial class Program;
