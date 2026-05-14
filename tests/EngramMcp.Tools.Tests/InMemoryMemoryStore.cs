using EngramMcp.Tools.Memory.Storage;

namespace EngramMcp.Tools.Tests;

public sealed class InMemoryMemoryStore(List<PersistedMemory> memories) : IMemoryStore
{
    public List<PersistedMemory> Memories { get; private set; } = memories;

    public void Replace(List<PersistedMemory> memories) => Memories = memories;

    public Task EnsureInitializedAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<List<PersistedMemory>> LoadAsync(CancellationToken cancellationToken = default) => Task.FromResult(Memories);

    public Task SaveAsync(IReadOnlyList<PersistedMemory> memories, CancellationToken cancellationToken = default)
    {
        Memories = memories.ToList();
        return Task.CompletedTask;
    }
}
