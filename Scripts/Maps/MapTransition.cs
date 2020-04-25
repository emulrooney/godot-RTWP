using Godot;
using System;

public class MapTransition : Node2D
{
	[Export] private string TransitionTo { get; set; }
	[Export] private bool IsWorldMapExit { get; set; }
	[Export] private Vector2 WorldMapCoordinates { get; set; }
	[Export] private bool RequireGatheredParty { get; set; }

	public void Transition(object body)
	{
		if (!(body.GetType() == typeof(PlayerCharacter)))
			return;

		Vector2? triggerLocation = null;

		if (RequireGatheredParty)
			triggerLocation = ((Node2D)body).Position;

		bool success;

		GM.UpdatePartyData();

		if (IsWorldMapExit)
			success = MapLoader.LoadMap("WORLDMAP", triggerLocation);
		else
			success = MapLoader.LoadMap(TransitionTo, triggerLocation);
	}

}
