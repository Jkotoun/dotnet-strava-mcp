# Strava MCP Server (.NET)

A personal learning project: building a [Model Context Protocol](https://modelcontextprotocol.io) (MCP) server in ASP.NET Core that exposes Strava data (activities, athlete stats, etc.) as tools an AI assistant can call.

**Why this exists:** this is a personal project for refreshing my .NET skills and learning newer .NET/ASP.NET Core features - minimal APIs, top-level statements, and so on - along with understanding how MCP actually works under the hood. Strava already publishes an [official MCP server](https://github.com/strava/strava-mcp-server) you could use instead; this one is built from scratch on purpose, including hand-rolling the JSON-RPC dispatch layer that a ready-made SDK normally hides.

## What MCP is, briefly

MCP is JSON-RPC 2.0 over a transport. This project targets **Streamable HTTP**: a single endpoint (`POST /mcp`) that a client sends JSON-RPC requests to (`initialize`, `tools/list`, `tools/call`, ...) and gets JSON-RPC responses back.

## Project structure

```
StravaMCP/
├── StravaMCP.sln
├── StravaMCP/                  # the server
│   ├── Program.cs              # MCP server wiring, maps POST /mcp
│   ├── Tools/MockTools.cs      # [McpServerTool] tool definitions
│   └── StravaMCP.http          # sample JSON-RPC requests (Rider / REST Client)
└── StravaMCP.Tests/             # contract-level integration tests
    ├── McpTestClient.cs        # thin JSON-RPC client over /mcp
    └── McpProtocolTests.cs
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Running locally

```bash
cd StravaMCP/StravaMCP
dotnet run --launch-profile http
```

The server listens on `http://localhost:5111`, with the MCP endpoint at `POST /mcp`. Try it with the requests in `StravaMCP.http`, or by hand:

```bash
curl -s http://localhost:5111/mcp \
  -X POST \
  -H "Content-Type: application/json" \
  -H "Accept: application/json, text/event-stream" \
  -d '{"jsonrpc":"2.0","id":1,"method":"tools/list"}'
```

## Running tests

```bash
cd StravaMCP
dotnet test
```

## Connecting a real MCP client (e.g. Claude Code)

With the server running:

```bash
claude mcp add --transport http --scope user strava-mcp http://localhost:5111/mcp
claude mcp list   # should show strava-mcp as Connected
```
