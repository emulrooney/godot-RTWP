using Godot;
using System;

public class AbilityToolbar : PanelContainer
{
    PlayerCharacter _pcOwner;
    
    private void OnForceAttackPressed()
    {
        GD.Print($"Force Attack by {_pcOwner}");
    }


    private void OnForceHaltPressed()
    {
        GD.Print($"Force Halt by {_pcOwner}");
    }


    private void OnAbilityPressed(int abilityIndex)
    {
        GD.Print($"Ability #{abilityIndex} Press by {_pcOwner}");
    }


    private void OnItemPressed(int itemIndex)
    {
	    GD.Print($"Ability #{itemIndex} Press by {_pcOwner}");
    }

}