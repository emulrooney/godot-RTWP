using Godot;
using System;
using System.Collections.Generic;

public class AOEAbility : Ability
{
	[Export] private bool IsTargeted { get; set; }
	[Export] private bool FriendlyFire { get; set; }

	[Export] public Vector2 targetLocation;


	protected Area2D targetArea;

	protected Particles2D onImpact;
	protected Timer onImpactTimer;

	List<Character> impactResults = new List<Character>();

	public override void _Ready()
	{
		base._Ready();

		targetArea = (Area2D)GetNodeOrNull("TargetArea");
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
		impactResults.Clear();
		targetArea.Position = targetLocation;

		foreach (var hit in targetArea.GetOverlappingBodies())
		{
			if (typeof(Character).IsAssignableFrom(hit.GetType()))
			{
				var character = (Character)hit;
				//Friendly fire gets ALL targets no matter what
				//Otherwise, non-allied targets are hit
				if (FriendlyFire || character.Faction != Caster.Faction)
					impactResults.Add(character);
			}
            else
            {
                GD.Print($"Invalid type: hit was {hit.GetType()}");
            }

		}

		TopPrinter.Two = $"Cast at: {targetArea.GlobalPosition}";
		TopPrinter.Three = $"Found: {impactResults.Count}";



	}

}
