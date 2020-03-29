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

    public void ClickAction(ClickInfo info, Vector2 location)
    {
        if (info.ButtonNumber == 1)
            MapCharacterManager.SelectPartyMember(this, !info.ModifyHeld);

    }
}
