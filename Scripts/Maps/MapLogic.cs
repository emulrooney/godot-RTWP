using Godot;
using System;
using System.Collections.Generic;

public class MapLogic : Area2D, IMapClickable
{
	Navigation2D nav = null;

	public List<EntryPoint> EntryPoints { get; private set; } = new List<EntryPoint>();
	[Export] public int ValidEntryDistance { get; private set; } = 48; //Allowable distance from player 0 on spawn

	public override void _Ready()
	{
		nav = (Navigation2D)GetNodeOrNull("Navigation2D");
		MapLoader.GetCurrentMap(true);

		//Called here due to timing issues w the MapLoader class
		MapLoader.FocusCameraAtEntry();

		GM.AddPartyToMap(this);
	}

	public void ClickAction(ClickInfo info, Vector2 location)
	{
		LocalCharacterManager.MapClick(ToLocal(location), nav);
	}

	public void RegisterEntryPoint(EntryPoint ep)
	{
		EntryPoints.Add(ep);
	}

	public bool PlaceCharacter(Character character, int epIndex = 0)
	{
		character.Position = EntryPoints[epIndex].GetNextFreePoint();
		
		return true;
	}

}
