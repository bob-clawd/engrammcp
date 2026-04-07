namespace EngramMcp.Tools.Memory;

public enum RetentionTier
{
    Short,
    Medium,
    Long
}

public static class RetentionRules
{
    extension(RetentionTier retentionTier)
    {
        internal double ToValue() => retentionTier switch
        {
            RetentionTier.Short => 5,
            RetentionTier.Medium => 25,
            RetentionTier.Long => 100,
            _ => throw new ArgumentOutOfRangeException(nameof(retentionTier), retentionTier, "Unsupported retention tier.")
        };
    }

    extension(double retention)
    {
        internal double Decay() => retention - 1;
        internal double Reinforce() => retention * 1.1;
        internal bool ShouldDelete() => retention < 1;
    }
}
