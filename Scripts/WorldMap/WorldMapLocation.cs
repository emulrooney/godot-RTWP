using Godot;
using System;

public class WorldMapLocation : Sprite
{
    [Export] public string LocationName { get; set; }
    private Area2D area;

    public override void _Ready()
    {
        area = (Area2D)GetNode("Area2D");
    }

    private void LocationEntered(object entered)
    {
        try
        {
            var enteredToken = (WorldMapToken)entered;
            if (enteredToken.IsPlayer)
            {
                GD.Print($"You can now enter: {LocationName}");
            }
        }
        catch (Exception e)
        {
        }
    }

}
