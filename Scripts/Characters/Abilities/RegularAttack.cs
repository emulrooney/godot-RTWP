using Godot;
using System;
using System.Collections.Generic;

public class RegularAttack : Area2D
{
    public bool ready = true;
    private static RandomNumberGenerator RNG = new RandomNumberGenerator();

    [Export] public int MinDamage { get; set; } = 1;
    [Export] public int MaxDamage { get; set; } = 8;
    [Export] public int AccuracyBonus { get; set; } = 0;
    [Export] public float Cooldown { get; set; } = 2f;
    public List<Character> InRange { get; set; } = new List<Character>();

    Timer attackTimer;

    public override void _Ready()
    {
        attackTimer = (Timer)GetNode("Timer");
    }

    public virtual int Attack()
    {
        attackTimer.SetWaitTime(Cooldown);
        return CalculateAttack(); 
    }

    private int CalculateAttack()
    {
        return RNG.RandiRange(MinDamage, MaxDamage);
    }


    private void CharacterEntered(int body_id, object body, int body_shape, int area_shape)
    {   
        //Called by signal on body entering Area2D
        if (body is Character)
            InRange.Add((Character)body);
    }

    private void CharacterExited(int body_id, object body, int body_shape, int area_shape)
    {
        //Called by signal on body exiting Area2D
        if (body is Character && InRange.Contains((Character)body))
            InRange.Remove((Character)body);
    }

    public bool CanAttack(Character target)
    {
        return (ready && CanReach(target));
    }

    public bool CanReach(Character target)
    {
        return InRange.Contains(target);
    }

    public void Ready()
    {
        ready = true;
    }
}

