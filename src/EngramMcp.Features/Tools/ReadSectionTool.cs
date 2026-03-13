using System.ComponentModel;
using EngramMcp.Core.Abstractions;
using ModelContextProtocol.Server;

namespace EngramMcp.Features.Tools;

public sealed class ReadSectionTool(IMemoryService memoryService) : Tool
{
    private const string ErrorHeading = "# Memory Section Error";

    [McpServerTool(Name = "read_section", Title = "Read Section", ReadOnly = true, Idempotent = true)]
    [Description("Read a single memory section by name. Works for built-in and custom sections using case-insensitive section lookup.")]
    public async Task<string> ExecuteAsync(
        [Description("The memory section to read.")]
        string section,
        CancellationToken cancellationToken)
    {
        try
        {
            var document = await memoryService.ReadAsync(section, cancellationToken).ConfigureAwait(false);

            return document.ToSectionMarkdown();
        }
        catch (ArgumentException exception) when (string.Equals(exception.ParamName, "section", StringComparison.Ordinal))
        {
            return $"{ErrorHeading}\r\nInvalid section identifier. Provide a non-empty section name.\r\n";
        }
        catch (KeyNotFoundException exception)
        {
            return $"{ErrorHeading}\r\nSection not found. {exception.Message}\r\n";
        }
        catch (Exception)
        {
            return $"{ErrorHeading}\r\nInternal failure. Unable to read the requested memory section right now.\r\n";
        }
    }
}
