using Godot;
using System;

public class AttackState : CharacterState
{
    private Timer timer;
    public bool Done { get; set; }

    public override void _Ready()
    {
        timer = (Timer)GetNode("Timer");
        timer.SetWaitTime(StateLength);
    }

    public override void OnStart()
    {
        Done = false;
        timer.SetWaitTime(StateLength);
        timer.Start();
    }

    public override void OnFinish()
    {
        NextAnimation();
    }

    public void Timeout()
    {
        GD.Print("Timeout!");
        Done = true;
        timer.SetWaitTime(StateLength);        
    }

}
