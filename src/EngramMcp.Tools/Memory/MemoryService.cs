using EngramMcp.Tools.Memory.Storage;

namespace EngramMcp.Tools.Memory;

public sealed class MemoryService(
    IMemoryStore memoryStore,
    RetentionCycle retentionCycle)
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<IReadOnlyList<RecallMemory>> RecallAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var memories = await memoryStore.LoadAsync(cancellationToken).ConfigureAwait(false);

            if (PruneDeleteableMemories(memories))
                await memoryStore.SaveAsync(memories, cancellationToken).ConfigureAwait(false);

            retentionCycle.Reset();
            return RecallMemories(memories);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<MemoryChangeResult> RememberAsync(RetentionTier retentionTier, string text, CancellationToken cancellationToken = default)
    {
        if (MemoryText.GetValidationError(text) is { } error)
            return MemoryChangeResult.Reject(error);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var memories = await memoryStore.LoadAsync(cancellationToken).ConfigureAwait(false);
            var memory = new PersistedMemory
            {
                Id = IdGenerator.GetUniqueId(),
                Text = text,
                Retention = retentionTier.ToValue()
            };

            memories.Add(memory);

            await memoryStore.SaveAsync(memories, cancellationToken).ConfigureAwait(false);
            return MemoryChangeResult.Success();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<MemoryChangeResult> ReinforceAsync(IReadOnlyList<string> memoryIds, CancellationToken cancellationToken = default)
    {
        if (memoryIds.Count == 0)
            return MemoryChangeResult.Reject("At least one memory id is required.");

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var memories = await memoryStore.LoadAsync(cancellationToken).ConfigureAwait(false);

            if (GetUnknownMemoryId(memoryIds, memories) is { } unknownMemoryId)
                return MemoryChangeResult.Reject($"Unknown memory '{unknownMemoryId}'.");

            var changedRetention = ApplyGlobalWeakeningIfFirstTime(memories);
            changedRetention |= ReinforceMemories(memories, memoryIds);

            if (changedRetention)
                await memoryStore.SaveAsync(memories, cancellationToken).ConfigureAwait(false);

            return MemoryChangeResult.Success();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<MemoryChangeResult> ForgetAsync(IReadOnlyList<string> memoryIds, CancellationToken cancellationToken = default)
    {
        if (memoryIds.Count == 0)
            return MemoryChangeResult.Reject("At least one memory id is required.");

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var memories = await memoryStore.LoadAsync(cancellationToken).ConfigureAwait(false);

            if (GetUnknownMemoryId(memoryIds, memories) is { } unknownMemoryId)
                return MemoryChangeResult.Reject($"Unknown memory '{unknownMemoryId}'.");

            var requestedMemoryIds = memoryIds.ToHashSet(StringComparer.Ordinal);
            var removedCount = memories.RemoveAll(memory => requestedMemoryIds.Contains(memory.Id));
            if (removedCount > 0)
                await memoryStore.SaveAsync(memories, cancellationToken).ConfigureAwait(false);

            return MemoryChangeResult.Success();
        }
        finally
        {
            _gate.Release();
        }
    }

    private static bool PruneDeleteableMemories(List<PersistedMemory> memories) =>
        memories.RemoveAll(memory => memory.Retention.ShouldDelete()) > 0;

    private static IReadOnlyList<RecallMemory> RecallMemories(IReadOnlyList<PersistedMemory> memories) =>
        memories
            .OrderByDescending(memory => memory.Retention)
            .Select(memory => new RecallMemory(memory.Id, memory.Text))
            .ToArray();

    private static string? GetUnknownMemoryId(IReadOnlyList<string> memoryIds, IReadOnlyList<PersistedMemory> memories)
    {
        var knownMemoryIds = memories
            .Select(memory => memory.Id)
            .ToHashSet(StringComparer.Ordinal);

        return memoryIds.FirstOrDefault(memoryId => string.IsNullOrWhiteSpace(memoryId) || !knownMemoryIds.Contains(memoryId));
    }

    private bool ApplyGlobalWeakeningIfFirstTime(List<PersistedMemory> memories)
    {
        if (!retentionCycle.CanDecay())
            return false;

        for (var index = 0; index < memories.Count; index++)
        {
            var memory = memories[index];
            memories[index] = memory with { Retention = memory.Retention.Decay() };
        }

        return true;
    }

    private bool ReinforceMemories(List<PersistedMemory> memories, IReadOnlyList<string> memoryIds)
    {
        var requestedMemoryIds = memoryIds.ToHashSet(StringComparer.Ordinal);
        var changedRetention = false;

        for (var index = 0; index < memories.Count; index++)
        {
            var memory = memories[index];

            if (!requestedMemoryIds.Contains(memory.Id) || !retentionCycle.CanReinforce(memory.Id))
                continue;

            memories[index] = memory with { Retention = memory.Retention.Reinforce() };
            changedRetention = true;
        }

        return changedRetention;
    }
}
