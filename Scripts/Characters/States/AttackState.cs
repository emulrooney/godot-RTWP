using Godot;
using System;

public class AttackState : CharacterState
{
	private Timer timer;
	public bool Done { get; set; }

	public override void _Ready()
	{
		timer = (Timer)GetNode("Timer");
		timer.WaitTime = StateLength;
	}

	public override void OnStart()
	{
		Done = false;
		timer.WaitTime = StateLength;
		timer.Start();
	}

	public override void OnFinish()
	{
		NextAnimation();
	}

	public void Timeout()
	{
		Done = true;
		timer.WaitTime = StateLength;        
	}

}
