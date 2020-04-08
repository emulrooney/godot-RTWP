using Godot;
using System;

public class PlayerCharacter : Character, IMapClickable
{ 
	[Export] public Texture Portrait { get; set; }
	[Export] public Color PortraitColor { get; set; }

	private RegularAttack weapon;

	public override void _Ready()
	{
		base._Ready();
		GUIManager.RegisterPlayerCharacter(this);

		weapon = (RegularAttack)GetNodeOrNull("EquippedWeapon");
	}

	public void ClickAction(ClickInfo info, Vector2 location)
	{
		if (info.ButtonNumber == 1)
			MapCharacterManager.SelectPartyMember(this, !info.ModifyHeld);

	}
}
