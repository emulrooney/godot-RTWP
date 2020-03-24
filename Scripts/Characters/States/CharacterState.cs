using Godot;
using System;

public abstract class CharacterState : Node
{
    public Character StateOwner { get; set; }
    [Export] public string AnimationName { get; set; } = "idle";
    [Export] public CharState StateType { get; set; }
    [Export] public float StateLength { get; set; } = -1; //If positive, FSM will run a timer and end
    public bool Complete { get; set; } //Only used for cases like "Attack"

    public abstract void OnStart();

    public abstract void OnFinish();

}
