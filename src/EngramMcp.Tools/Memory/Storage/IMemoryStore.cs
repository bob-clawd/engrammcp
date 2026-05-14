namespace EngramMcp.Tools.Memory.Storage;

public interface IMemoryStore
{
    Task EnsureInitializedAsync(CancellationToken cancellationToken = default);
    Task<List<PersistedMemory>> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(IReadOnlyList<PersistedMemory> memories, CancellationToken cancellationToken = default);
}
