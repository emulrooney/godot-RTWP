using Godot;
using System;

public abstract class Ability : Node2D
{
    protected Character Caster { get; set; }

    [Export] private bool isItemAbility = false;
    [Export] private bool isChargedAbility = false;

    [Export] public string AbilityName { get; set; }
    [Export] public Texture ToolbarIcon { get; set; }
    [Export] public Color IconColor { get; set; }
    [Export] public string AnimationName { get; set; }

    protected Particles2D OnCast;
    protected Particles2D WhileActive; 

    public override void _Ready()
    {
        Caster = (Character)GetParent<Character>();
        Caster.Abilities.Add(this);

        OnCast = (Particles2D)GetNodeOrNull("OnCast");
        WhileActive = (Particles2D)GetNodeOrNull("WhileActive");
    }

    public virtual bool Use() {
        CombatLog.UseAbility($"{Caster} casts {AbilityName}.");

        if (OnCast != null)
            OnCast.Emitting = true;

        if (WhileActive != null)
            WhileActive.Emitting = true;

        return true;
    }

    public virtual void Complete()
    {
        WhileActive.Emitting = false;
    }

    public virtual void EndModifier(StatblockModifier modifier)
    {
        Caster.Stats.RemoveModifier(modifier);
    }
}
