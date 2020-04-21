using Godot;
using System;

public class TargetedAbilityInfo
{
    public TargetedAbility Ability { get; set; }
    public Character Caster { get; set; }
    public bool Complete { get; set; }

    public bool IsValid
    {
        get
        {
            if (Complete || Caster.IsDead || !Caster.CanUseAbility(Ability))
                return false;

            return true;
        }
    }

    public TargetedAbilityInfo(Character caster, TargetedAbility ability)
    {
        Caster = caster;
        Ability = ability;
        Complete = false;
    }



}
