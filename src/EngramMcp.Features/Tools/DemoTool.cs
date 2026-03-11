using System.ComponentModel;
using ModelContextProtocol.Server;

namespace EngramMcp.Features.Tools;

public sealed class DemoTool<T> : Tool
{
    [McpServerTool(Name = "demo", Title = "Demo", ReadOnly = true, Idempotent = true)]
    [Description("Use this tool when ...")]
    public Task ExecuteAsync(CancellationToken cancellationToken,
        [Description("Exact path to a project file (.csproj). Specify only one of projectPath, projectName, or projectId.")]
        string? projectPath = null,
        [Description("Name of a project. Specify only one of projectPath, projectName, or projectId.")]
        string? projectName = null,
        [Description(
            "Project identifier from the current loaded workspace snapshot. projectId values are snapshot-local and can change after reload, so prefer projectPath for durable automation. Specify only one of projectPath, projectName, or projectId.")]
        string? projectId = null,
        [Description("Filter to only types in namespaces starting with this prefix.")]
        string? namespacePrefix = null,
        [Description("Filter by type kind: class, record, interface, enum, or struct.")]
        string? kind = null,
        [Description("Filter by accessibility: public, internal, protected, private, protected_internal, or private_protected.")]
        string? accessibility = null,
        [Description("When true, includes XML documentation summaries for returned type entries when available. Defaults to false.")]
        bool? includeSummary = null,
        [Description(
            "When true, includes a lightweight preview of declared members for each returned type entry. This is not full member metadata: each member is returned as a single normalized accessibility-plus-signature string, and only members declared on that type are included. Enrichment is applied only to the type entries returned on the current page. Use list_members as the detailed follow-up tool. Defaults to false.")]
        bool? includeMembers = null,
        [Description("Maximum number of results to return. Defaults to 100, maximum 500.")]
        int? limit = null,
        [Description("Number of results to skip for pagination. Defaults to 0.")]
        int? offset = null
    )
    {
        return Task.CompletedTask;
    }
}
