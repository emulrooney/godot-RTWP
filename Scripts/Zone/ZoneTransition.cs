using Godot;
using System;

public class ZoneTransition : Node2D
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
		if (IsWorldMapExit)
			success = ZoneLoader.LoadMap("WORLDMAP", triggerLocation);
		else
			success = ZoneLoader.LoadMap(TransitionTo, triggerLocation);

		if (!success)
			GD.Print("Couldn't load map!");
	}

}
