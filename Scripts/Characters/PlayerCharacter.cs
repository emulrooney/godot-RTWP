using Godot;
using System;

public class PlayerCharacter : Character, IMapClickable
{
    private RegularAttack weapon;

    public override void _Ready()
    {
        base._Ready();
        weapon = (RegularAttack)GetNodeOrNull("EquippedWeapon");
    }

    public void ClickAction(Vector2 location)
    {
        //TODO determine use for click action

        if (Input.IsMouseButtonPressed(1))
            MapCharacterManager.AddCharacterToSelected(this);
    }
}
