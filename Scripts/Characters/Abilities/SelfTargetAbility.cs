using Godot;
using System;

public class SelfTargetAbility : Ability
{
	[Export] private StatType affectedStat;
	[Export] private int powerLevel = 1;

	public override void _Ready()
	{
		base._Ready();
	}

	public override void Release()
	{
		ApplyActiveVisuals();
		ApplyModifier(Caster, affectedStat, powerLevel);
	}

	public void EndEffect()
	{
		whileActive.Emitting = false;
	}

}
