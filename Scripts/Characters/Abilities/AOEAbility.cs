using Godot;
using System;
using System.Collections.Generic;

public class AOEAbility : Ability
{
	[Export] private bool FriendlyFire { get; set; }

	protected Area2D rangeArea;
	protected Area2D targetArea;

	protected Particles2D onImpact;
	protected Timer onImpactTimer;

	public override Vector2 TargetLocation
	{
		get
		{
			return targetArea.Position;
		}
		set
		{
			targetArea.GlobalPosition = value;
			onImpact.GlobalPosition = value;
		}
	}

	[Export] public int accuracy = 20;
	[Export] public int powerLevel = 30;
	[Export] public int dieSidesPerPowerLevel = 2;


	List<Character> impactResults = new List<Character>();

	public override void _Ready()
	{
		base._Ready();

		targetArea = (Area2D)GetNodeOrNull("TargetArea");
		rangeArea = (Area2D)GetNodeOrNull("RangeArea");
		onImpact = (Particles2D)GetNodeOrNull("OnImpact");
		onImpactTimer = (Timer)GetNodeOrNull("OnImpact/Timer");

		if (targetArea == null)
		{
			GD.Print($"No target area found for {AbilityName} for {Caster.Name}. Disabling ability.");
			QueueFree();
		}
	}

	public override void Release()
	{
		impactResults = GetTargetsAt(TargetLocation);
		
		if (onImpact != null)
		{
			onImpact.Position = TargetLocation;
			onImpact.Emitting = true;
		}

		foreach (var affected in impactResults)
		{

			affected.ReceiveAttack(RNG.Next(1, 100) + accuracy, RNG.Next(1, dieSidesPerPowerLevel + 1) * powerLevel, 1);
		}
	}

	public virtual List<Character> GetTargetsAt(Vector2 location)
	{
		List<Character> targets = new List<Character>();
		targetArea.Position = location;

		foreach (var hit in targetArea.GetOverlappingBodies())
		{
			if (typeof(Character).IsAssignableFrom(hit.GetType()))
			{
				var character = (Character)hit;
				//Friendly fire gets ALL targets no matter what
				//Otherwise, non-allied targets are hit
				if (FriendlyFire || character.Faction != Caster.Faction)
					targets.Add(character);
			}
		}

		return targets;
	}

}
