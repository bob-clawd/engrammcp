using EngramMcp.Tools.Memory.Storage;

namespace EngramMcp.Tools.Tests;

public sealed class InMemoryMemoryStore(PersistedMemoryDocument document) : IMemoryStore
{
    public PersistedMemoryDocument Document { get; private set; } = document;

    public void Replace(PersistedMemoryDocument document) => Document = document;

    public Task EnsureInitializedAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<PersistedMemoryDocument> LoadAsync(CancellationToken cancellationToken = default) => Task.FromResult(Document);

    public Task SaveAsync(PersistedMemoryDocument document, CancellationToken cancellationToken = default)
    {
        Document = document;
        return Task.CompletedTask;
    }
}
