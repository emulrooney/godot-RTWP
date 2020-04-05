using Godot;
using System;

public class WorldMapLogic : Node2D
{ 
    WorldMapToken PlayerToken { get; set; }
    Navigation2D navigation;

    public override void _Ready()
    {
        PlayerToken = (WorldMapToken)GetNodeOrNull("Player");
        navigation = (Navigation2D)GetNode("Navigation2D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

        if (Input.GetMouseButtonMask() == 1)
        {
            PlayerToken.SetTarget(navigation.GetSimplePath(PlayerToken.Position, GetGlobalMousePosition()));
        }    
    }
}
