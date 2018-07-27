
public enum Facing
{
	UpRight,
    DownRight,
    DownLeft,
    UpLeft
}

public enum UnitState
{
    SelectingMoveTarget,
    Moving,
    SelectingActionTarget,
    Attacking,
    EndingPhase
}

public enum CursorState
{
    SelectingUnit,
    SelectingDestination,
    SelectingTarget,
    Inactive
}

public enum UnitClass
{
    Warrior,
    Hunter
}