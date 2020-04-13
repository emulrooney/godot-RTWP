using Godot;
using System;
using System.Collections.Generic;

public abstract class Character : KinematicBody2D
{
    [Export] public string CharacterName { get; set; }
    [Export] public bool IsDead { get; protected set; }

    //MOVEMENT
    public static float TargetTolerance { get; set; } = 3f;
    [Export] public CharFaction Faction { get; set; } = CharFaction.NEUTRAL;

    public Queue<Vector2[]> QueuedMoves { get; set; } = new Queue<Vector2[]>();
    public Character AttackTarget { get; set; }

    public Statblock Stats { get; private set; }

    protected CharacterAnimator Animator { get; set; }
    private RegularAttack RegularAttack { get; set; }

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

        ((FSMachine)GetNode("FSMachine")).Activate(); //Setup once stats set
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


        var roll = Stats.AccuracyRoll;
        var damage = Stats.BaseDamage + RegularAttack.Attack();

        CombatLog.Attack(this.Name, target.Name, roll);
        target.ReceiveAttack(roll, damage);
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
                CombatLog.Hit(Name, damage);
            }
            else
            {
                CombatLog.Miss();
            }

            if (Stats.CurrentHP <= 0)
                Die();
        }
    }

    public void SetSelectionCircle(bool visibility)
    {
        Animator.SetSelectionCircleOn(visibility);
    }

    protected virtual void Die()
    {
        CombatLog.Death(Name);
        IsDead = true;
    }

}