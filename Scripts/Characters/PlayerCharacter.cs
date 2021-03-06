using Godot;
using System;
using System.Collections.Generic;

public class PlayerCharacter : Character, IMapClickable
{
	public int PartyMemberOrder { get; set; } = -1;

	[Export] public Texture Portrait { get; set; }
	[Export] public Color PortraitColor { get; set; }
	[Export] public SpriteFrames AnimationSet { get; set; }

	private RegularAttack weapon;

	public override void _Ready()
	{
		base._Ready();
		PartyMemberOrder = GUIManager.RegisterPlayerCharacter(this);

		weapon = (RegularAttack)GetNodeOrNull("EquippedWeapon");
		SetSelectionCircle(false);

		if (AnimationSet != null)
			Animator.Frames = AnimationSet;
	}

	public void ClickAction(ClickInfo info, Vector2 location)
	{
		LocalCharacterManager.SelectPartyMember(this, !info.ModifyHeld);
		GUIManager.SelectPartyMember(PartyMemberOrder);
	}

	//public override bool UseAbility(int index)
	//{
	//    if (CanUseAbility(Abilities[index]))
	//    {
	//        var ability = Abilities[index];
	//        if (ability.IsTargeted)
	//            InputManager.SetAimingAbility(new TargetedAbilityInfo(this, ability));
	//        else
	//            QueuedAbility = Abilities[index];
	//        return true;
	//    }
	//    else
	//        return false;
	//}

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

    public void LoadData(PartyMemberData data)
    {
        CharacterName = data.CharacterName;
        Portrait = data.Portrait;
        PortraitColor = data.PortraitColor;
        AnimationSet = data.AnimationSet;

        Stats = (Statblock)GetNode("Statblock");
        Stats.LoadData(data);

        GUIManager.UpdateFor(this);
    }

}
