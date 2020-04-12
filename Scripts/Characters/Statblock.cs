using Godot;
using System;

public class Statblock : Node
{
    private static RandomNumberGenerator RNG = new RandomNumberGenerator();

    [Export] public int CurrentHP { get; set; }
    [Export] public int MaxHP { get; set; }

    [Export] public float MoveSpeed { get; set; } = 88;

    [Export] public int BaseAccuracy { get; set; }
    [Export] public int BaseDamage { get; set; }
    [Export] public int BaseDefense { get; set; }

    public int AccuracyRoll { get => RNG.RandiRange(0, 100) + BaseAccuracy; }

    public int Defense { get => BaseDefense; }

}
