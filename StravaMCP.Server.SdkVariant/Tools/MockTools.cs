using System.ComponentModel;
using ModelContextProtocol.Server;

namespace StravaMCP.Tools;

[McpServerToolType]
public static class MockTools
{
    [McpServerTool, Description("Echoes back the provided message.")]
    public static string Echo(string message) => message;

    [McpServerTool, Description("Adds two numbers together.")]
    public static int AddNumbers(int a, int b) => a + b;
}
