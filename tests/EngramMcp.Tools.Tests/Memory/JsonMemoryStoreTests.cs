using System.Text.Json;
using EngramMcp.Tools.Memory.Storage;
using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Memory;

public sealed class JsonMemoryStoreTests
{
    [Fact]
    public async Task EnsureInitializedAsync_creates_empty_memory_document()
    {
        using var memoryFile = new TemporaryMemoryFile();
        var store = new JsonMemoryStore(memoryFile.FilePath);

        await store.EnsureInitializedAsync();

        File.Exists(memoryFile.FilePath).IsTrue();

        using var document = JsonDocument.Parse(await File.ReadAllTextAsync(memoryFile.FilePath));
        document.RootElement.TryGetProperty("memories", out var memories).IsTrue();
        memories.ValueKind.Is(JsonValueKind.Array);
        memories.GetArrayLength().Is(0);
    }

    [Fact]
    public async Task SaveAsync_persists_memory_entries_to_json_file()
    {
        using var memoryFile = new TemporaryMemoryFile();
        var store = new JsonMemoryStore(memoryFile.FilePath);

        await store.SaveAsync(new PersistedMemoryDocument([
            new PersistedMemory { Id = "260329142501", Text = "Durable fact", Retention = 10 }
        ]));

        var json = await File.ReadAllTextAsync(memoryFile.FilePath);

        json.Contains("Durable fact", StringComparison.Ordinal).IsTrue();
        json.Contains("\"id\": \"260329142501\"", StringComparison.Ordinal).IsTrue();
    }

    [Fact]
    public async Task SaveAsync_orders_memories_by_retention_descending_then_id_ascending()
    {
        using var memoryFile = new TemporaryMemoryFile();
        var store = new JsonMemoryStore(memoryFile.FilePath);

        await store.SaveAsync(new PersistedMemoryDocument([
            new PersistedMemory { Id = "weak", Text = "Weak", Retention = 1 },
            new PersistedMemory { Id = "200", Text = "Newer strong", Retention = 10 },
            new PersistedMemory { Id = "middle", Text = "Middle", Retention = 5 },
            new PersistedMemory { Id = "100", Text = "Older strong", Retention = 10 },
        ]));

        using var document = JsonDocument.Parse(await File.ReadAllTextAsync(memoryFile.FilePath));
        var memoryIds = document.RootElement
            .GetProperty("memories")
            .EnumerateArray()
            .Select(memory => memory.GetProperty("id").GetString())
            .ToArray();

        memoryIds.Is(["100", "200", "middle", "weak"]);
    }

    [Fact]
    public async Task LoadAsync_reads_existing_memories_from_json_file()
    {
        using var memoryFile = new TemporaryMemoryFile();

        await File.WriteAllTextAsync(memoryFile.FilePath, """
        {
          "memories": [
            {
              "id": "260329142501",
              "text": "Remember project detail",
              "retention": 10
            }
          ]
        }
        """);

        var store = new JsonMemoryStore(memoryFile.FilePath);

        var document = await store.LoadAsync();

        document.Memories.Count.Is(1);
        document.Memories[0].Id.Is("260329142501");
        document.Memories[0].Text.Is("Remember project detail");
        document.Memories[0].Retention.Is(10d);
    }
}
