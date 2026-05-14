using System.Text.Json;
using EngramMcp.Tools.Memory.Storage;
using Is.Assertions;
using Xunit;

namespace EngramMcp.Tools.Tests.Memory;

public sealed class JsonlMemoryStoreTests
{
    [Fact]
    public async Task EnsureInitializedAsync_creates_empty_memory_file()
    {
        using var memoryFile = new TemporaryMemoryFile();
        var store = new JsonlMemoryStore(memoryFile.FilePath);

        await store.EnsureInitializedAsync();

        File.Exists(memoryFile.FilePath).IsTrue();
        (await File.ReadAllTextAsync(memoryFile.FilePath)).Is(string.Empty);
    }

    [Fact]
    public async Task SaveAsync_persists_memory_entries_as_jsonl()
    {
        using var memoryFile = new TemporaryMemoryFile();
        var store = new JsonlMemoryStore(memoryFile.FilePath);

        await store.SaveAsync(new PersistedMemoryDocument([
            new PersistedMemory { Id = "260329142501", Text = "Durable fact", Retention = 10 }
        ]));

        var lines = await File.ReadAllLinesAsync(memoryFile.FilePath);

        lines.Length.Is(1);
        lines[0].Contains("Durable fact", StringComparison.Ordinal).IsTrue();
        lines[0].Contains("\"id\":\"260329142501\"", StringComparison.Ordinal).IsTrue();
    }

    [Fact]
    public async Task SaveAsync_orders_memories_by_retention_descending_then_id_ascending()
    {
        using var memoryFile = new TemporaryMemoryFile();
        var store = new JsonlMemoryStore(memoryFile.FilePath);

        await store.SaveAsync(new PersistedMemoryDocument([
            new PersistedMemory { Id = "weak", Text = "Weak", Retention = 1 },
            new PersistedMemory { Id = "200", Text = "Newer strong", Retention = 10 },
            new PersistedMemory { Id = "middle", Text = "Middle", Retention = 5 },
            new PersistedMemory { Id = "100", Text = "Older strong", Retention = 10 },
        ]));

        var lines = await File.ReadAllLinesAsync(memoryFile.FilePath);
        var memoryIds = lines
            .Select(line => JsonDocument.Parse(line).RootElement.GetProperty("id").GetString())
            .ToArray();

        memoryIds.Is(["100", "200", "middle", "weak"]);
    }

    [Fact]
    public async Task LoadAsync_reads_existing_memories_from_jsonl_file()
    {
        using var memoryFile = new TemporaryMemoryFile();

        await File.WriteAllLinesAsync(memoryFile.FilePath,
        [
            "{\"id\":\"260329142501\",\"text\":\"Remember project detail\",\"retention\":10}",
            "{\"id\":\"260329142502\",\"text\":\"Another detail\",\"retention\":5}"
        ]);

        var store = new JsonlMemoryStore(memoryFile.FilePath);

        var document = await store.LoadAsync();

        document.Memories.Count.Is(2);
        document.Memories[0].Id.Is("260329142501");
        document.Memories[0].Text.Is("Remember project detail");
        document.Memories[0].Retention.Is(10d);
    }

    [Fact]
    public async Task LoadAsync_migrates_legacy_json_document_to_jsonl()
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

        var store = new JsonlMemoryStore(memoryFile.FilePath);

        var document = await store.LoadAsync();
        var lines = await File.ReadAllLinesAsync(memoryFile.FilePath);

        document.Memories.Count.Is(1);
        lines.Length.Is(1);
        lines[0].StartsWith("{\"id\":", StringComparison.Ordinal).IsTrue();
    }
}
