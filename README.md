# Strava MCP Server (.NET)

A personal learning project: building a [Model Context Protocol](https://modelcontextprotocol.io) (MCP) server in ASP.NET Core that exposes Strava data (activities, athlete stats, etc.) as tools an AI assistant can call.

**Why this exists:** this is a personal project for refreshing my .NET skills and learning newer .NET/ASP.NET Core features - minimal APIs, top-level statements, and so on - along with understanding how MCP actually works under the hood. Strava already publishes an [official MCP server](https://github.com/strava/strava-mcp-server) you could use instead; this one is built from scratch on purpose, including hand-rolling the JSON-RPC dispatch layer that a ready-made SDK normally hides.

## What MCP is, briefly

MCP is JSON-RPC 2.0 over a transport. This project targets **Streamable HTTP**: a single endpoint (`POST /mcp`) that a client sends JSON-RPC requests to (`initialize`, `tools/list`, `tools/call`, ...) and gets JSON-RPC responses back.

## Project structure

- **`StravaMCP.Strava`** — shared Strava OAuth token client + API wrapper, no ModelContextProtocol dependency
- **`StravaMCP.Server.SdkVariant`** — MCP server built with the official ModelContextProtocol SDK
- **`StravaMCP.Server.FromScratchVariant`** — MCP server with a hand-rolled JSON-RPC dispatcher, no SDK
- **`StravaMCP.Strava.Tests`** — unit tests for the shared library (no real network calls)
- **`StravaMCP.Tests.Common`** — shared test infra (`McpTestClient`, fake Strava HTTP boundary)
- **`StravaMCP.Tests`** / **`StravaMCP.Server.FromScratchVariant.Tests`** — the same contract tests run against each variant

`SdkVariant` and `FromScratchVariant` are two independent, non-collaborating implementations of the same MCP server, kept side by side on purpose for comparison/learning — not a pipeline where one depends on the other. Both are exercised by the *same* contract-test assertions (`StravaMCP.Tests` / `StravaMCP.Server.FromScratchVariant.Tests`, sharing `McpTestClient` from `StravaMCP.Tests.Common`), so they double as an ongoing parity suite proving both behave identically.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Configuration & secrets

.NET has no `.env`/`.env.example` equivalent. Instead: `appsettings.json` holds the non-secret shape (committed), and real secret values only ever go into [`dotnet user-secrets`](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) (never committed, stored outside the repo).

The `Strava` config section:

```json
"Strava": {
  "ClientId": "",
  "ClientSecret": "",
  "RefreshToken": "",
  "ApiBaseUrl": "https://www.strava.com/api/v3/",
  "TokenUrl": "https://www.strava.com/oauth/token"
}
```

- `ApiBaseUrl` / `TokenUrl` are safe, working defaults for everyone — already committed.
- `ClientId` is a public identifier (it's visible in plain sight in the OAuth authorize URL), safe to commit.
- `ClientSecret` and `RefreshToken` are real secrets — leave them empty in `appsettings.json` and set them via user-secrets instead:

To get your own values:
1. Register an app at [strava.com/settings/api](https://www.strava.com/settings/api) → note the `Client ID` / `Client Secret`.
2. Visit `https://www.strava.com/oauth/authorize?client_id=<ID>&redirect_uri=http://localhost/exchange_token&response_type=code&scope=read,activity:read_all` in a browser, approve, and copy the `code` query param from the (unreachable, that's fine) redirect URL.
3. Exchange it once for a refresh token:
   ```bash
   curl -X POST https://www.strava.com/oauth/token \
     -d client_id=<ID> -d client_secret=<SECRET> \
     -d code=<CODE> -d grant_type=authorization_code
   ```
4. From `StravaMCP.Server.SdkVariant`:
   ```bash
   dotnet user-secrets init   # once per project
   dotnet user-secrets set "Strava:ClientSecret" "<secret>"
   dotnet user-secrets set "Strava:RefreshToken" "<refresh_token from step 3>"
   ```
   `StravaMCP.Server.FromScratchVariant` intentionally shares the same `UserSecretsId` as `SdkVariant` (set directly in its `.csproj`), so both variants read the same secrets file — no need to redo this setup a second time for the same Strava account.

## Running locally

Either variant works the same way, just on different ports:

```bash
cd StravaMCP.Server.SdkVariant          # http://localhost:5111
dotnet run --launch-profile http
```

```bash
cd StravaMCP.Server.FromScratchVariant  # http://localhost:5155
dotnet run --launch-profile http
```

The MCP endpoint is `POST /mcp` on both. Try `SdkVariant` with the requests in its `StravaMCP.http`, or by hand (works against either port — `FromScratchVariant` returns plain JSON, `SdkVariant` returns SSE-framed JSON, hence accepting both content types):

```bash
curl -s http://localhost:5111/mcp \
  -X POST \
  -H "Content-Type: application/json" \
  -H "Accept: application/json, text/event-stream" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list"}'
```

## Running tests

```bash
dotnet test
```

## Connecting a real MCP client (e.g. Claude Code)

With the server running:

```bash
claude mcp add --transport http --scope user strava-mcp http://localhost:5111/mcp
claude mcp list   # should show strava-mcp as Connected
```
