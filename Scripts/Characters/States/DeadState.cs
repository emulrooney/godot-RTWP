using Godot;
using System;

public class DeadState : CharacterState
{

	private Timer timer;
	[Export] private bool queueFreeOnDeath = true;

	public override void _Ready()
	{
		if (queueFreeOnDeath)
		{
			timer = (Timer)GetNodeOrNull("Timer");

            if (timer != null)
    			timer.WaitTime = StateLength;
            else
            {
                GD.Print($"No timer for queueFree for {StateOwner.Name}. Disabled queueFree.");
                queueFreeOnDeath = false;
            }
		}
	}

	public override void OnStart()
	{
		if (queueFreeOnDeath)
		{
			timer.WaitTime = StateLength;
			timer.Start();
		}
	}

	public override void OnFinish()
	{
	}

	public void Timeout()
	{
		((FSMachine)Owner).OwnerQueueFree();
	}


}
