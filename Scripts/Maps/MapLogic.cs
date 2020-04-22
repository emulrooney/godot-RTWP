using Godot;
using System;

public class MapLogic : Area2D, IMapClickable
{
	Navigation2D nav = null;
	[Export] public Vector2[] EntryPoints { get; private set; }

	public override void _Ready()
	{
		nav = (Navigation2D)GetNodeOrNull("Navigation2D");
		MapLoader.GetCurrentMap(true);
	}

	public void ClickAction(ClickInfo info, Vector2 location)
	{
		LocalCharacterManager.MapClick(ToLocal(location), nav);
	}

}
