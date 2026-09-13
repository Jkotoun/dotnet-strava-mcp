namespace StravaMCP.Server.FromScratchVariant.Mcp;

public sealed class ToolRegistry(IEnumerable<IMcpTool> tools)
{
    private readonly Dictionary<string, IMcpTool> _tools = tools.ToDictionary(tool => tool.Name);

    public IReadOnlyCollection<IMcpTool> All => _tools.Values;

    public IMcpTool? TryGet(string name) => _tools.GetValueOrDefault(name);
}
