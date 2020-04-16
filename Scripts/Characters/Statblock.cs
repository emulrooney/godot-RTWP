using Godot;
using System;
using System.Collections.Generic;

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

    public int Defense
    {
        get
        {
            var returnDefense = BaseDefense;
            for (int i = 0; i < Modifiers.Count; i++)
                if (Modifiers[i].StatModified == StatType.DEFENSE)
                    returnDefense += Modifiers[i].Amount;

            return returnDefense;
        }
    }
    private List<StatblockModifier> Modifiers { get; set; } = new List<StatblockModifier>();

    public void AddModifier(StatblockModifier modifier)
    {
        GD.Print("Applied mod...");
        Modifiers.Add(modifier);
    }


    public void RemoveModifier(StatblockModifier modifier)
    {
        var removed = Modifiers.Remove(modifier);
    }

}
