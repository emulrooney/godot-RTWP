using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Statblock representation for map character. Shared btwn players and monsters
/// </summary>
public class Statblock : Node
{
    /* RAW STATS */
    [Export] public int CurrentHP { get; set; }
    [Export] public int MaxHP { get; set; }

    [Export] public float MoveSpeed { get; set; } = 88;

    [Export] public int BaseAccuracy { get; set; }
    [Export] public int BaseDamage { get; set; }
    [Export] public int BaseDefense { get; set; }

    /* DERIVED STATS */
    public int Damage { get => BaseDamage + GetAllModifiersFor(StatType.DAMAGE); }
    public int Accuracy { get => BaseAccuracy + GetAllModifiersFor(StatType.ACCURACY); }
    public int Defense { get => BaseDefense + GetAllModifiersFor(StatType.DEFENSE); }

    /* STAT MODIFICATIONS */

    private List<StatblockModifier> Modifiers { get; set; } = new List<StatblockModifier>();

    public void Heal(int amount)
    {
        CurrentHP += amount;
        CurrentHP = Math.Min(CurrentHP, MaxHP);
    }

    public int GetAllModifiersFor(StatType modifierType)
    {
        var returnVal = 0;

        for (int i = 0; i < Modifiers.Count; i++)
            if (Modifiers[i].StatModified == modifierType)
            {
                returnVal += Modifiers[i].Amount;
            }

        return returnVal;
    }

    public void AddModifier(StatblockModifier modifier)
    {
        Modifiers.Add(modifier);
    }

    public void RemoveModifier(StatblockModifier modifier)
    {
        Modifiers.Remove(modifier);
    }

    /* MISCELLANEOUS */

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public void LoadData(PartyMemberData data)
    {
        CurrentHP = data.CurrentHP;
        MaxHP = data.MaxHP;
        MoveSpeed = data.MoveSpeed;
        BaseAccuracy = data.BaseAccuracy;
        BaseDamage = data.BaseDamage;
        BaseDefense = data.BaseDefense;
    }

    public int Roll(StatType stat)
    {
        int statValue = 0;

        switch (stat)
        {
            case StatType.ACCURACY:
                statValue = Accuracy;
                break;
            case StatType.DEFENSE:
                statValue = Defense;
                break;
            default:
                throw new NotSupportedException();
        };

        return GM.RNG.RandiRange(0, 100) + statValue;
    }

}
