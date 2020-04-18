using Godot;
using System;

public abstract class Ability : Node2D
{
    protected Character Caster { get; set; }
    public bool IsCharged { get; private set; }
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
                GD.Print($"Invalid charging settings for {Name}! Disabling 'whileCharging' effects.");
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
        CombatLog.UseAbility($"{Caster.CharacterName} casts {AbilityName}.");
        
        if (!IsCharged && onCast != null)
        {
            onCast.Emitting = true;
            Release();
        }
    }

    /// <summary>
    /// Spell effects are applied.
    /// </summary>
    public virtual void Release()
    {
    }

    /// <summary>
    /// Called when charged spell is finished charging. Calls 'Release' to apply effects, then ends
    /// </summary>
    public virtual void ChargeComplete()
    {
        if (onCast != null)
            onCast.Emitting = true;

        if (whileActive != null)
            whileActive.Emitting = true;
    }

    protected void ApplyModifier(Character target, StatType affectedStat, int powerLevel)
    {
        var modifier = new StatblockModifier(this, affectedStat, powerLevel, ActiveTime);
        target.Stats.AddModifier(modifier);
    }

    public virtual void EndModifier(StatblockModifier modifier)
    {
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
