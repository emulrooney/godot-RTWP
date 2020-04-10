using Godot;
using System;

/// <summary>
/// Enum for states used by FSMachine
/// </summary>
public enum FSMState
{
    Idle,
    Move,
    MoveAttack,
    Attack,
    Dead
}
