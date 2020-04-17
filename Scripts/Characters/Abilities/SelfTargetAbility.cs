using Godot;
using System;

public class SelfTargetAbility : Ability
{
	[Export] private StatType affectedStat;
	[Export] private int powerLevel = 1;
	[Export] private float affectLengthInSeconds = 0;


	public override bool Use()
	{
		base.Use();
		ApplyModifier();

		return true;

	}

	private void ApplyModifier()
	{
		var modifier = new StatblockModifier(this, affectedStat, powerLevel, affectLengthInSeconds * 1000);
		Caster.Stats.AddModifier(modifier);
	}

}
