using Godot;
using System;

public class PlayerCharacter : Character, IMapClickable
{ 
	[Export] public Texture Portrait { get; set; }
	[Export] public Color PortraitColor { get; set; }
	[Export] public SpriteFrames AnimationSet { get; set; }

	private RegularAttack weapon;

	public override void _Ready()
	{
		base._Ready();
		GUIManager.RegisterPlayerCharacter(this);

		weapon = (RegularAttack)GetNodeOrNull("EquippedWeapon");
		SetSelectionCircle(false);

		if (AnimationSet != null)
			Animator.Frames = AnimationSet;
	}

	public void ClickAction(ClickInfo info, Vector2 location)
	{
		if (info.ButtonNumber == 1)
			LocalCharacterManager.SelectPartyMember(this, !info.ModifyHeld);
	}

	public override void ReceiveAttack(int hitRoll, int damage, int damageType = 0)
	{
		if (!IsDead)
		{
			base.ReceiveAttack(hitRoll, damage, damageType);
			GUIManager.UpdateFor(this);
		}
	}

	public void ForceHalt()
	{
		AttackTarget = null;
		QueuedMoves.Clear();
	}

	protected override void Die()
	{
		base.Die();
		GUIManager.UpdateFor(this);
	}
}
