using Godot;
using System;

public class SelfTargetAbility : Ability
{
	private Character Caster { get; set; }

	public override void _Ready()
	{
		Caster = (Character)GetParent<Character>();
		Caster.Abilities.Add(this);
	}

	public override bool Use()
	{
		GD.Print($"{Caster.Name} attempted to cast: {Name} on themselves!");
		return true;
	}
}
