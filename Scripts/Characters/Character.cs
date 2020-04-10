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

    private CharacterAnimator Animator { get; set; }
    private RegularAttack RegularAttack { get; set; }

    public override void _Ready()
    {
        LocalCharacterManager.RegisterPresent(this);
        RegularAttack = GetNodeOrNull<RegularAttack>("RegularAttack");
        Animator = GetNodeOrNull<CharacterAnimator>("Animator");

        Stats = GetNode<Statblock>("Statblock"); //No statblock, no bueno
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

        target.ReceiveAttack(roll, damage);
        GD.Print($"{this.Name} attacked {target.Name} : Acc {roll}, Damage {damage}");
    }


    public virtual void ReceiveAttack(int hitRoll, int damage, int damageType = 0)
    {
        if (!IsDead)
        {
            int defenseRoll = 0; // stats.DefenseRoll;
            int result = hitRoll - defenseRoll;

            if (result > 50)
            {
                //Hit!
                Stats.CurrentHP -= damage;
                Animator.OnHit();
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
        IsDead = true;
    }

}