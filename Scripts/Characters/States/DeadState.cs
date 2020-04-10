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
			timer = (Timer)GetNode("Timer");
			timer.WaitTime = StateLength;
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
