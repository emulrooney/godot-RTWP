using Godot;
using System;
using System.Collections.Generic;

public abstract class Ability : Node2D
{
    protected static Random RNG = new Random();
    protected Character Caster { get; set; }
    public bool IsCharged { get; private set; }
    
    public bool IsTargeted { get; protected set; } //syntactic sugar. only set by 'TargetedAbility'
    
    [Export] private bool IsItemAbility { get; set; } = false; //todo
    [Export] public FSMState[] AllowedInStates { get; private set; } = new FSMState[] //default
    {
        FSMState.Idle,
        FSMState.Move,
        FSMState.MoveAttack,
        FSMState.Attack
    };

    public List<AbilityEffect> Effects { get; protected set; } = new List<AbilityEffect>();

    //Visuals
    [Export] public string AbilityName { get; set; }
    [Export] public Texture ToolbarIcon { get; set; }
    [Export] public Color IconColor { get; set; }
    [Export] public string AnimationName { get; set; }

    //charging
    [Export] public float ChargeTime { get; private set; } = -1f; // > 0 == charged
    protected Particles2D whileCharging;
    private Timer whileChargingTimer;

    //cast
    protected Particles2D onCast;

    //active
    [Export] public float ActiveTime { get; private set; }
    protected Particles2D whileActive;
    private Timer whileActiveTimer;

    //Stats applied to ALL effects
    [Export] public int accuracy = 20;
    [Export] public int powerLevel = 30;
    [Export] public int dieSidesPerPowerLevel = 2;

    public override void _Ready()
    {
        var effects = GetNode("Effects");
        for (int i = 0; i < effects.GetChildCount(); i++)
        {
            Effects.Add(effects.GetChild<AbilityEffect>(i));
        }

        if (Effects.Count == 0)
        {
            GD.Print($"No spell effects on {AbilityName}! This ability will do nothing when used!");
        }

        Caster = GetParent<Character>();
        Caster.Abilities.Add(this);

        //charging
        if (ChargeTime > 0)
        {
            IsCharged = true;
            whileCharging = (Particles2D)GetNodeOrNull("WhileCharging");
            whileChargingTimer = (Timer)GetNodeOrNull("WhileCharging/Timer");

            if (whileCharging == null || whileChargingTimer == null)
            {
                whileCharging = null;
                whileChargingTimer = null;
            }
        }

        //cast
        onCast = (Particles2D)GetNodeOrNull("OnCast");

        //active
        if (ActiveTime > 0)
        {
            whileActive = (Particles2D)GetNodeOrNull("WhileActive");
            whileActiveTimer = (Timer)GetNodeOrNull("WhileActive/Timer");

            if (whileActive == null || whileActiveTimer == null)
            {
                GD.Print($"Invalid active settings for {Name}! Disabling 'whileActive' effects.");
                whileActive = null;
                whileActiveTimer = null;
            }
        }

    }

    /// <summary>
    /// Called when ability is first activated. 
    /// </summary>
    public virtual void Use() {
        if (!IsCharged)
        {
            CombatLog.UseAbility($"{Caster.CharacterName} casts {AbilityName}.");

            if (onCast != null)
                onCast.Emitting = true;

            if (whileActive != null)
                whileActive.Emitting = true;

            Release();
        }
    }

    /// <summary>
    /// Spell effects are applied.
    /// </summary>
    public abstract void Release();

    /// <summary>
    /// Called when charged spell is finished charging. Calls 'Release' to apply effects, then ends
    /// </summary>
    public virtual void ChargeComplete()
    {
        CombatLog.UseAbility($"{Caster.CharacterName} casts {AbilityName}.");

        if (onCast != null)
            onCast.Emitting = true;

        if (whileActive != null)
            whileActive.Emitting = true;

        Release();
    }

    protected void DamageHealth(Character target, AbilityEffect effect)
    {
        target.ReceiveAttack(
            RNG.Next(1, 100) + accuracy + effect.BonusAccuracy,
            RNG.Next(powerLevel + effect.BonusFlatValue, (dieSidesPerPowerLevel + effect.BonusSides + 1)) * (powerLevel + effect.BonusDie), 
            1
        );
    }

    protected void RestoreHealth(Character target, AbilityEffect effect)
    {
        target.ReceiveHeal(powerLevel + effect.BonusFlatValue);
    }

    protected void ApplyModifier(Character target, AbilityEffect effect)
    {
        var modifier = new StatblockModifier(this, effect.StatType, powerLevel + effect.BonusFlatValue, ActiveTime + effect.BonusTime);
        GD.Print($"Applying {modifier.StatModified} mod: {modifier.Amount} increase to {target.Name}");
        target.Stats.AddModifier(modifier);
    }

    public virtual void EndModifier(StatblockModifier modifier)
    {
        GD.Print($"Removing ({modifier.StatModified} + {modifier.Amount}) as cast by {Caster.Name}");
        Caster.Stats.RemoveModifier(modifier);
    }

    protected void ApplyAbilityEffectsTo(Character target)
    {
        foreach (var effect in Effects)
        {
            switch (effect.Type)
            {
                case AbilityEffectType.DAMAGE:
                    DamageHealth(target, effect);
                    break;
                case AbilityEffectType.HEALING:
                    RestoreHealth(target, effect);
                    break;
                case AbilityEffectType.STAT_MOD:
                    ApplyModifier(target, effect);
                    break;
                default:
                    break;
            }
        }
    }

    protected void ApplyActiveVisuals()
    {
        if (whileActive != null && whileActiveTimer != null)
        {
            whileActive.Emitting = true;
            whileActiveTimer.Start(ActiveTime);
        }
    }

}
