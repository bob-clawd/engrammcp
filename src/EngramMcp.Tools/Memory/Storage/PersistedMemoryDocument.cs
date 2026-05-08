namespace EngramMcp.Tools.Memory.Storage;

public sealed class PersistedMemoryDocument
{
    public List<PersistedMemory> Memories { get; init; } = [];

    public void SortMemoriesByDescendingRetention() =>
        Memories.Sort((left, right) => right.Retention.CompareTo(left.Retention));
}
