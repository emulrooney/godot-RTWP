using Godot;
using System;
using System.Collections.Generic;

public abstract class Character : KinematicBody2D
{

    [Export] public string CharacterName { get; set; }

    //MOVEMENT
    public static float TargetTolerance { get; set; } = 3f;
    [Export] public CharFaction Faction { get; set; } = CharFaction.NEUTRAL;

    public Queue<Vector2[]> QueuedMoves { get; set; } = new Queue<Vector2[]>();
    public Character AttackTarget { get; set; }

    protected Statblock stats;
    private RegularAttack RegularAttack { get; set; }

    public override void _Ready()
    {
        MapCharacterManager.RegisterPresent(this);
        RegularAttack = (RegularAttack)GetNodeOrNull("RegularAttack");
        stats = (Statblock)GetNode("Statblock");
    }

    public bool CanAttackTarget(Character target)
    {
        if (RegularAttack != null)
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

        TopPrinter.Four = $"Path target: {QueuedMoves.Peek()[0]}";
    }

    public void Attack(Character target)
    {
        var roll = stats.AccuracyRoll;
        var damage = stats.BaseDamage + RegularAttack.Attack();

        target.ReceiveAttack(roll, damage);
        GD.Print($"{this.Name} attacked {target.Name} : Acc {roll}, Damage {damage}");
    }


    public void ReceiveAttack(int hitRoll, int damage, int damageType = 0)
    {
        GD.Print($"{this.Name} -- I was hit!");

        int defenseRoll = 0; // stats.DefenseRoll;
        int result = hitRoll - defenseRoll;

        if (result > 50)
            stats.CurrentHP -= damage; 

        if (stats.CurrentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        MapCharacterManager.UnregisterPresent(this);
    }

}