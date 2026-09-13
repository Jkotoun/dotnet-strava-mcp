var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Phase 5: hand-rolled JSON-RPC dispatch (own DTOs, tool registry, McpDispatcher) goes here - no
// ModelContextProtocol SDK reference in this project by design.

app.Run();

public partial class Program;
