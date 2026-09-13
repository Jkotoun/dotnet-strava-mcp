namespace StravaMCP.Server.FromScratchVariant.Mcp;

public sealed class ToolRegistry
{
    private readonly Dictionary<string, IMcpTool> _tools;

    public ToolRegistry(IEnumerable<IMcpTool> tools)
    {
        _tools = tools.ToDictionary(tool => tool.Name);
    }

    public IReadOnlyCollection<IMcpTool> All => _tools.Values;

    public IMcpTool? TryGet(string name) => _tools.GetValueOrDefault(name);
}
