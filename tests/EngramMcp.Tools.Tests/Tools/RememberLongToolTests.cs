using EngramMcp.Tools.Memory.Retention;
using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Tools;

public sealed class RememberLongToolTests
{
    [Fact]
    public async Task ExecuteAsync_stores_long_term_memory()
    {
        var memoryService = new ToolTestMemoryService();
        var tool = new EngramMcp.Tools.Tools.RememberLong.McpTool(memoryService);

        var response = await tool.ExecuteAsync("Remember this");

        response.Is("Stored long-term memory.");
        memoryService.RememberedTier.Is(RetentionTier.Long);
        memoryService.RememberedText.Is("Remember this");
    }
}
