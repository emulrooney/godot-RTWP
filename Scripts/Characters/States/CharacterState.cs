using Godot;
using System;

public abstract class CharacterState : Node
{
    public Character StateOwner { get; set; }

    public string AnimationName { get
        {
            return AnimationNames[currentAnimation];
        }
    }

    protected int currentAnimation = 0;
        
    [Export] public string[] AnimationNames { get; set; } = { "idle" };
    [Export] public CharState StateType { get; set; }
    [Export] public float StateLength { get; set; } = -1; //If positive, FSM will run a timer and end
    public bool Complete { get; set; } //Only used for cases like "Attack"

    public abstract void OnStart();

    public abstract void OnFinish();

    protected void NextAnimation()
    {
        currentAnimation++;
        if (currentAnimation > AnimationNames.Length - 1)
            currentAnimation = 0;
    }

}
