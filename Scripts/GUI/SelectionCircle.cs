using Godot;
using System.Collections.Generic;

public class SelectionCircle : Sprite
{
    private static Dictionary<CharFaction, Color> colors = new Dictionary<CharFaction, Color>()
    {
        { CharFaction.ALLY, new Color(0.3f, 0.8f, 1f) },
        { CharFaction.NEUTRAL, new Color(1f, 1f, 1f) },
        { CharFaction.ENEMY, new Color(1f, .2f, .2f) }
    };

    public override void _Ready()
    {
        var parent = (Character)GetParent();
        Modulate = (colors[parent.Faction]);
    }

}