using Godot;
using System;

public class MapLogic : Area2D, IMapClickable
{
	Navigation2D nav = null;

	public override void _Ready()
	{
		nav = (Navigation2D)GetNodeOrNull("Navigation2D");
	}

	public void ClickAction(ClickInfo info, Vector2 location)
	{
		MapCharacterManager.MapClick(ToLocal(location), nav);
	}

}
