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

		if (affectedStat == StatType.HP)
		{
			RestoreHealth(Caster, powerLevel);

			if (Caster.GetType() == typeof(PlayerCharacter))
				GUIManager.UpdateFor((PlayerCharacter)Caster);
		}
		else if (affectedStat == StatType.MOVESPEED)
			CombatLog.Miss(); //Shouldn't be able to do this.
		else
			ApplyModifier(Caster, affectedStat, powerLevel);
	}

	public void EndEffect()
	{
		whileActive.Emitting = false;
	}

}
