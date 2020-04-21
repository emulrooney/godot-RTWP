using Godot;
using System;

public class SelfTargetAbility : Ability
{
	public override void _Ready()
	{
		base._Ready();
	}

	public override void Release()
	{
		ApplyActiveVisuals();
		ApplyAbilityEffectsTo(Caster);

		if (Caster.GetType() == typeof(PlayerCharacter))
			GUIManager.UpdateFor((PlayerCharacter)Caster);
	}

	public void EndEffect()
	{
		whileActive.Emitting = false;
	}

}
