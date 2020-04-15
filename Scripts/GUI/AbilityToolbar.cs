using Godot;
using System;

public class AbilityToolbar : PanelContainer
{
	PlayerCharacter _pcOwner;

	public override void _Ready()
	{
		GUIManager.RegisterElement(this);
	}

	public void UpdateFor(PlayerCharacter pc)
	{
		_pcOwner = pc;

		//Set abilities based on character
		//Set item based on character


	}
	
	private void OnForceAttackPressed()
	{
		GD.Print($"Force Attack by {_pcOwner}");
	}


	private void OnForceHaltPressed()
	{
		GD.Print($"Force Halt by {_pcOwner}");

		if (_pcOwner != null)
			_pcOwner.ForceHalt();

	}


	private void OnAbilityPressed(int abilityIndex)
	{
		GD.Print($"Ability #{abilityIndex} Press by {_pcOwner}");
	}


	private void OnItemPressed(int itemIndex)
	{
		GD.Print($"Item #{itemIndex} Press by {_pcOwner}");
	}

}
