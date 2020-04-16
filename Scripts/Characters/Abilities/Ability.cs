using Godot;
using System;

public abstract class Ability : Node2D
{
    protected Character Caster { get; set; }
    [Export] public string AbilityName { get; set; }
    [Export] public Texture ToolbarIcon { get; set; }
    [Export] public Color IconColor { get; set; }
    [Export] public string AnimationName { get; set; }

    protected Particles2D OnCast;

    public override void _Ready()
    {
        Caster = (Character)GetParent<Character>();
        Caster.Abilities.Add(this);

        OnCast = (Particles2D)GetNodeOrNull("OnCast");
    }

    public virtual bool Use() {
        CombatLog.UseAbility($"{Caster} casts {AbilityName}.");

        if (OnCast != null)
        {
            OnCast.Emitting = true;
        }

        return true;
    }
}
