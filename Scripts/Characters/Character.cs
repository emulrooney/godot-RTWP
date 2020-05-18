using Godot;
using System;
using System.Collections.Generic;

public abstract class Character : KinematicBody2D
{
	private FSMachine _fsm;
    private EventCreateObject onDeath;

    private Tween MoveTween { get; set; }

    public int MoveLeft { get; set; } = -1;


    public Vector2 GridPosition {
        get 
        {
            return GM.GetGridPosition(Position);
        }
        set
        {
            Position = GM.GetGridPosition(value);
        }
    }
	[Export] public string CharacterName { get; set; }
	[Export] public bool IsDead { get; protected set; }

	//MOVEMENT
	[Export] public CharFaction Faction { get; set; } = CharFaction.NEUTRAL;

    public Queue<Vector2> QueuedMoves { get; set; } = new Queue<Vector2>();
	public Character AttackTarget { get; set; } //TODO: Should this be a list? Maybe for enemy characters...
	public Ability QueuedAbility { get; set; } //TODO: Should this be a list? Worth considering...

	public Statblock Stats { get; protected set; }
	protected CharacterAnimator Animator { get; set; }
	private RegularAttack RegularAttack { get; set; }

	public List<Ability> Abilities { get; set; } = new List<Ability>();



	public override void _Ready()
	{
		LocalCharacterManager.RegisterPresent(this);
		RegularAttack = GetNodeOrNull<RegularAttack>("RegularAttack");
		Animator = GetNodeOrNull<CharacterAnimator>("Animator");
        MoveTween = GetNode<Tween>("MoveTween");

        GD.Print("Starting at " + Position);

        GridPosition = GM.GetGridPosition(Position);

        GD.Print("Moving to Grid Pos: " + GridPosition);

        Position = GridPosition * GM.TileSize;

        GD.Print("Position is now " + Position);



		//Check for an overrideStatblock -- this lets us set custom stats for pre-placed encounters
		Stats = GetNodeOrNull<Statblock>("OverrideStatblock");

		if (Stats == null)
			Stats = GetNode<Statblock>("Statblock"); //Use default statblock if no custom one found.
		else
			GetNode<Statblock>("Statblock").QueueFree(); //Remove the regular one, we'll use the override!

		_fsm = ((FSMachine)GetNode("FSMachine"));
        onDeath = (EventCreateObject)GetNodeOrNull("OnDeath");

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

    public virtual bool UseAbility(Ability ability)
    {
        if (CanUseAbility(ability))
        {
            QueuedAbility = ability;
            return true;
        }
        
        return false;
    }

    //Override used by AbilityIcons on toolbar
	public virtual bool UseAbility(int index)
	{
        return UseAbility(Abilities[index]);
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

    public void MoveTo(Vector2 mapLocation)
    {
        var gridLocation = GM.GetGridPosition(mapLocation);

        var path = GM.Map.GetPathInTiles(GridPosition, gridLocation);

        if (path != null && path.Length >= 1)
        {
            QueuedMoves.Clear();

            for (int i = 1; i < path.Length; i++)
            {
                QueuedMoves.Enqueue(path[i]);
            }

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
