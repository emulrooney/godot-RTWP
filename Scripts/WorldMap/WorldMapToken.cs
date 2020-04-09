using Godot;
using System;

public class WorldMapToken : KinematicBody2D
{
	[Export] public float TravelSpeed { get; set; } = 16f;
	[Export] public float TargetTolerance { get; set; }
	public Vector2[] TargetLocation { get; protected set; }
	private int targetLocationCounter = -1;

	public bool HasTarget { get; set; } = false;
	[Export] public bool IsPlayer { get; set; }

	public override void _PhysicsProcess(float delta)
	{
		if (HasTarget)
			MoveTowardsLocation();
	}

	public void SetTarget(Vector2[] targets)
	{
		if (targets.Length > 0)
		{
			TargetLocation = targets;
			targetLocationCounter = 0;
			HasTarget = true;
		}
	}

	private void MoveTowardsLocation()
	{
		if (targetLocationCounter < TargetLocation.Length)
		{
			if ((TargetLocation[targetLocationCounter] - Position).Length() > TargetTolerance)
			{
				Vector2 velocity = (TargetLocation[targetLocationCounter] - Position).Normalized() * TravelSpeed;
				MoveAndSlide(velocity);
			}
			else
				targetLocationCounter++;
		}
		else
		{
			targetLocationCounter = -1;
			HasTarget = false;
		}
	}
}
