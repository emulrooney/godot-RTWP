using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TargetedAbility : Ability
{
	protected Area2D rangeArea;
	protected Particles2D onImpact;
	protected Timer onImpactTimer;
	protected Particles2D projectile;
	protected Tween projectileTween;

	public Character TargetCharacter { get; set; }

	[Export] public float timeToTargetLength { get; set; } = -1;
	[Export] public int accuracy = 20;
	[Export] public int powerLevel = 30;
	[Export] public int dieSidesPerPowerLevel = 2;


	public override void _Ready()
	{
		base._Ready();

		rangeArea = (Area2D)GetNodeOrNull("RangeArea");
		onImpact = (Particles2D)GetNodeOrNull("OnImpact");
		onImpactTimer = (Timer)GetNodeOrNull("OnImpact/Timer");
		projectile = (Particles2D)GetNodeOrNull("Projectile");
		projectileTween = (Tween)GetNodeOrNull("Projectile/Tween");
	}

	public override void Release()
	{
		if (timeToTargetLength > -1 && projectile != null)
		{
			projectile.Emitting = true;
			projectileTween.InterpolateProperty(projectile, "position", Position, TargetLocation - GlobalPosition, timeToTargetLength);
			projectileTween.InterpolateCallback(this, timeToTargetLength, "Impact");
			projectileTween.Start();
		}
		else
		{
			Impact();
		}
	}

	public void Impact()
	{
		if (projectile != null)
			projectile.Emitting = false;

		if (onImpact != null)
		{
			onImpact.Position = TargetLocation;
			onImpact.Emitting = true;
		}
	}

	public async Task FlyTowards(Vector2 location)
	{
		float timeElapsed = 0;
		projectile.Emitting = true;
		while (timeElapsed < timeToTargetLength)
		{
			timeElapsed += GetPhysicsProcessDeltaTime() * 100;
			projectile.Position = Caster.Position.LinearInterpolate(TargetCharacter.Position, timeElapsed / timeToTargetLength);

			await ToSignal(GetTree(), "idle_frame");
		}

		projectile.Emitting = false;
	}
}
