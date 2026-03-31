namespace EngramMcp.Tools.Memory;

public sealed class Tracker
{
    private bool _decayed;
    private readonly HashSet<string> _reinforced = new(StringComparer.Ordinal);

    public void Reset()
    {
        _decayed = false;
        _reinforced.Clear();
    }

    public bool Decayed()
    {
        if (_decayed)
            return false;

        _decayed = true;
        return true;
    }

    public bool Reinforced(string memoryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memoryId);
        return _reinforced.Add(memoryId);
    }
}
