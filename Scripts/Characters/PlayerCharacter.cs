using Godot;
using System;

public class PlayerCharacter : Character
{
    private RegularAttack weapon;

    public override void _Ready()
    {
        base._Ready();
        weapon = (RegularAttack)GetNodeOrNull("EquippedWeapon");
    }

    private void ClickCharacter(object viewport, object @event, int shape_idx)
    {
        if (Input.IsMouseButtonPressed(1))
            MapCharacterManager.SelectCharacter(this);
    }
}
