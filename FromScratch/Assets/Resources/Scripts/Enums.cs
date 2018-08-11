
public enum Facing
{
	UpRight,
    DownRight,
    DownLeft,
    UpLeft
}

public enum UnitState
{
    SelectingDestination,
    Moving,
    AwaitingChoice,
    SelectingTarget,
    Attacking,
    EndingPhase
}

public enum CursorState
{
    SelectingUnit,
    SelectingDestination,
    AwaitingChoice,
    SelectingTarget,
    Inactive
}

public enum UnitClass
{
    Shieldmaiden,
    Warrior,
    Sentinel
}

public enum ActionButton
{
    Attack,
    Wait,
    Cripple,
    Impair,
    Weaken
}

public enum AttackType
{
    Cripple,
    Impair,
    Weaken
}