using Godot;
using System;

public abstract class Ability : Node2D
{
    protected Character Caster { get; set; }
    public bool IsCharged { get; private set; }

    [Export] public bool IsTargeted { get; private set; }
    public bool HasTarget { get; private set; } = false;
    public virtual Vector2 TargetLocation { get; set; }

    [Export] private bool IsItemAbility { get; set; } = false; //todo
    [Export] public FSMState[] AllowedInStates { get; private set; } = new FSMState[] //default
    {
        FSMState.Idle,
        FSMState.Move,
        FSMState.MoveAttack,
        FSMState.Attack
    };

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

    public override void _Ready()
    {
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
                GD.Print("Invalid active settings for {Name}! Disabling 'whileActive' effects.");
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

    protected void DamageHealth(Character target, int powerLevel)
    {

    }

    protected void RestoreHealth(Character target, int powerLevel)
    {
        target.ReceiveHeal(powerLevel);
    }

    protected void ApplyModifier(Character target, StatType affectedStat, int powerLevel)
    {
        GD.Print($"Applying ({affectedStat} + {powerLevel}) to {target.Name}");
        var modifier = new StatblockModifier(this, affectedStat, powerLevel, ActiveTime);
        target.Stats.AddModifier(modifier);
    }

    public virtual void EndModifier(StatblockModifier modifier)
    {
        GD.Print($"Removing ({modifier.StatModified} + {modifier.Amount}) as cast by {Caster.Name}");
        Caster.Stats.RemoveModifier(modifier);
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
