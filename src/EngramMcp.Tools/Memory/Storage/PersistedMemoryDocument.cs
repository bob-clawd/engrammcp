using System.Text.Json.Serialization;

namespace EngramMcp.Tools.Memory.Storage;

public sealed class PersistedMemoryDocument
{
    public PersistedMemoryDocument()
    { }

    public PersistedMemoryDocument(IEnumerable<PersistedMemory> memories)
    {
        ArgumentNullException.ThrowIfNull(memories);

        Memories = memories.ToList();
    }

    [JsonInclude]
    public List<PersistedMemory> Memories { get; private set; } = [];

    public void Sort() =>
        Memories = Memories
            .OrderByDescending(memory => memory.Retention)
            .ThenBy(memory => memory.Id, StringComparer.Ordinal)
            .ToList();
}
