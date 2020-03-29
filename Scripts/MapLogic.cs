using Godot;
using System;

public class MapLogic : Area2D, IMapClickable
{
    public void ClickAction(Vector2 location)
    {
        MapCharacterManager.MapClick(location);
    }

}