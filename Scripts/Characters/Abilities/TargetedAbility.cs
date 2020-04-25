using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TargetedAbility : Ability
{
	[Export] public bool CanHitGround { get; private set; }
	[Export] public bool CanHitCharacter { get; private set; }

	protected Area2D rangeArea;
	protected Particles2D onImpact;
	protected Timer onImpactTimer;
	protected Particles2D projectile;
	protected Tween projectileTween;

	public virtual Character TargetCharacter { get; protected set; }
	public virtual Vector2 TargetLocation { get; protected set; }

	[Export] public float timeToTargetLength { get; set; } = -1;

	public override void _Ready()
	{
		base._Ready();

		rangeArea = (Area2D)GetNodeOrNull("RangeArea");
		onImpact = (Particles2D)GetNodeOrNull("OnImpact");
		onImpactTimer = (Timer)GetNodeOrNull("OnImpact/Timer");
		projectile = (Particles2D)GetNodeOrNull("Projectile");
		projectileTween = (Tween)GetNodeOrNull("Projectile/Tween");

		IsTargeted = true;

		if (!CanHitCharacter && !CanHitGround)
		{
			GD.Print($"{AbilityName} can't hit anything! Toggling 'CanHitGround' back on.");
			CanHitGround = true;
		}
	}

	public override void Release()
	{
		if (timeToTargetLength > -1 && projectile != null)
		{
            var targetPos = (TargetCharacter == null || !Godot.Object.IsInstanceValid(TargetCharacter) ? TargetLocation : TargetCharacter.Position);

            projectile.Emitting = true;

			projectileTween.InterpolateProperty(projectile, "position", Position,
				targetPos - GlobalPosition,
				timeToTargetLength);

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

		if (Godot.Object.IsInstanceValid(TargetCharacter) && !TargetCharacter.IsDead)
			ApplyAbilityEffectsTo(TargetCharacter);
	}

	/// <summary>
	/// Sets end point of targeted ability's effect. Uses IMapClickable interface to allow the map or 
	/// various characters to be targeted.
	/// </summary>
	/// <param name="target"></param>
	public virtual bool SetTarget(IMapClickable target)
	{
		bool successfulTarget = false;

		switch (target)
		{
			case MapLogic ml:
				if (CanHitGround)
				{
					TargetCharacter = null;
					TargetLocation = GetGlobalMousePosition();
					successfulTarget = true;
				}
				break;
			case Character c:
				if (CanHitCharacter)
				{
					TargetCharacter = c;
					successfulTarget = true;
				}
				break;
			default:
				GD.Print($"Something went wrong clicking on {target}");
				break;
		}

		return successfulTarget;
	}
}
