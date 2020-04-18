using Godot;
using System;
using System.Collections.Generic;

public abstract class Character : KinematicBody2D
{
	private FSMachine _fsm;
	[Export] public string CharacterName { get; set; }
	[Export] public bool IsDead { get; protected set; }

	//MOVEMENT
	public static float TargetTolerance { get; set; } = 3f;
	[Export] public CharFaction Faction { get; set; } = CharFaction.NEUTRAL;

	public Queue<Vector2[]> QueuedMoves { get; set; } = new Queue<Vector2[]>();
	public Character AttackTarget { get; set; } //TODO: Should this be a list? Maybe for enemy characters...
	public Ability QueuedAbility { get; set; } //TODO: Should this be a list? Worth considering...

	public Statblock Stats { get; private set; }

	protected CharacterAnimator Animator { get; set; }
	private RegularAttack RegularAttack { get; set; }

	public List<Ability> Abilities { get; set; } = new List<Ability>();

	public override void _Ready()
	{
		LocalCharacterManager.RegisterPresent(this);
		RegularAttack = GetNodeOrNull<RegularAttack>("RegularAttack");
		Animator = GetNodeOrNull<CharacterAnimator>("Animator");


		//Check for an overrideStatblock -- this lets us set custom stats for pre-placed encounters
		Stats = GetNodeOrNull<Statblock>("OverrideStatblock");

		if (Stats == null)
			Stats = GetNode<Statblock>("Statblock"); //Use default statblock if no custom one found.
		else
			GetNode<Statblock>("Statblock").QueueFree(); //Remove the regular one, we'll use the override!

		_fsm = ((FSMachine)GetNode("FSMachine"));
		_fsm.Activate(); //Setup once stats set
	}

	public bool CanAttackTarget(Character target)
	{
		if (!target.IsDead && RegularAttack != null)
			return RegularAttack.CanAttack(target);

		return false;
	}

	public bool CanReachTarget(Character target)
	{
		if (RegularAttack != null)
		{
			return RegularAttack.CanReach(target);
		}

		return false;
	}

	public bool CanUseAbility(Ability ability)
	{
		if (Abilities.Contains(ability))
		{
			for (int i = 0; i < ability.AllowedInStates.Length; i++)
				if (ability.AllowedInStates[i] == _fsm.Current.StateType)
					return true;
		}

		GD.Print("Can't use right now!");
		return false;
	}


	public void AddToPath(Vector2[] location, bool enqueueNextAction = false)
	{
		if (!enqueueNextAction)
		{
			QueuedMoves = new Queue<Vector2[]>();
			AttackTarget = null;
		}

		QueuedMoves.Enqueue(location);
	}

	public void Attack(Character target)
	{
		if (target.IsDead)
		{
			AttackTarget = null;
			return;
		}


		var roll = Stats.Roll(StatType.ACCURACY);
		var damage = Stats.Damage + RegularAttack.Attack();

		CombatLog.Attack(this.CharacterName, target.CharacterName, roll);
		target.ReceiveAttack(roll, damage);
	}

	public bool UseAbility(int index)
	{
		if (CanUseAbility(Abilities[index]))
		{
			QueuedAbility = Abilities[index];
			return true;
		}
		else
			return false;
	}

	public virtual void ReceiveAttack(int hitRoll, int damage, int damageType = 0)
	{
		if (!IsDead)
		{
			int result = hitRoll - Stats.Defense;

			if (result > 50)
			{
				//Hit!
				Stats.CurrentHP -= damage;
				Animator.OnHit();
				CombatLog.Hit(CharacterName, damage);
			}
			else
			{
				CombatLog.Miss();
			}

			if (Stats.CurrentHP <= 0)
				Die();
		}
	}

    public virtual void ReceiveHeal(int healed)
    {
        if (!IsDead)
        {
            Stats.Heal(healed);
            CombatLog.Heal(CharacterName, healed);
        }
    }

	public void SetSelectionCircle(bool visibility)
	{
		Animator.SetSelectionCircleOn(visibility);
	}

	protected virtual void Die()
	{
		CombatLog.Death(CharacterName, this.GetType() == typeof(PlayerCharacter));
		IsDead = true;
	}

}
