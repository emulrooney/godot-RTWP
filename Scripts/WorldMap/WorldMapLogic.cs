using Godot;
using System;

public class WorldMapLogic : Node2D
{ 
    WorldMapToken PlayerToken { get; set; }

    public override void _Ready()
    {
        PlayerToken = (WorldMapToken)GetNodeOrNull("Player");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.GetMouseButtonMask() == 1)
        {
            PlayerToken.SetTarget(GetGlobalMousePosition());
        }    
    }
}
