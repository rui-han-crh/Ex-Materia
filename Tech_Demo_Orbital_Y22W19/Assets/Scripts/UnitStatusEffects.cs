public struct UnitStatusEffects
{
    public enum Status
    {
        Overwatch
    }

    public readonly bool OnOverwatch;

    public UnitStatusEffects(UnitStatusEffects oldStatuses, bool onOverwatch)
    {
        this = (UnitStatusEffects) oldStatuses.MemberwiseClone();
        this.OnOverwatch = onOverwatch;
    }

    public UnitStatusEffects ApplyStatus(Status effect)
    {
        return effect switch
        {
            Status.Overwatch => new UnitStatusEffects(this, true),
            _ => this,
        };
    }

    public UnitStatusEffects RemoveStatus(Status effect)
    {
        return effect switch
        {
            Status.Overwatch => new UnitStatusEffects(this, false),
            _ => this,
        };
    }

    public override string ToString()
    {
        return $"Overwatch: {OnOverwatch} |";
    }
}