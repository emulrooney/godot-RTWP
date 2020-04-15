using Godot;
using System;

public class SelfTargetAbility : Node2D, IAbility
{
	private Character Caster { get; set; }

	[Export] public string AbilityName { get; set; }
	[Export] public Texture ToolbarIcon { get; set; }
	[Export] public Color IconColor { get; set; }

	public override void _Ready()
	{
		Caster = (Character)GetParent<Character>();
		Caster.Abilities.Add(this);
	}

	public bool Use()
    {
        GD.Print($"{Caster.Name} attempted to cast: {Name} on themselves!");
		return true;
	}
}
