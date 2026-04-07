namespace EngramMcp.Tools.Memory;

public sealed class RetentionCycle
{
    private bool _decayed;
    
    private readonly HashSet<string> _reinforced = new(StringComparer.Ordinal);

    public void Reset()
    {
        _decayed = false;
        
        _reinforced.Clear();
    }

    public bool CanDecay()
    {
        var result = !_decayed;
        
        _decayed = true;
        
        return result;
    }

    public bool CanReinforce(string memoryId) => _reinforced.Add(memoryId);
}
