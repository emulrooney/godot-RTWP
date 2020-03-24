using Godot;
using System;

public class MapLogic : Area2D
{
    /// <summary>
    /// Triggered on click on map
    /// </summary>
    /// <param name="viewport"></param>
    /// <param name="event"></param>
    /// <param name="shape_idx"></param>
    private void MouseClick(object viewport, object @event, int shape_idx)
    {
        if (Input.IsMouseButtonPressed(1))
        {
            MapCharacterManager.MouseClick();
        }
    }

}