using Godot;
using System;

public abstract class Ability : Node2D
{
    [Export] public string AbilityName { get; set; }
    [Export] public Texture ToolbarIcon { get; set; }
    [Export] public Color IconColor { get; set; }

    public abstract bool Use();
}
