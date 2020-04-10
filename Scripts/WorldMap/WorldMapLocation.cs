using Godot;
using System;

public class WorldMapLocation : Sprite
{
	[Export] public string LocationName { get; set; }
    [Export] public string LocationKey { get; set; }
	private Area2D area;

	public override void _Ready()
	{
		area = (Area2D)GetNode("Area2D");
	}

	private void LocationEntered(object entered)
	{
        if (entered.GetType() == typeof(WorldMapToken))
        {
			var enteredToken = (WorldMapToken)entered;
			if (enteredToken.IsPlayer)
			{
                ZoneLoader.LoadMap(LocationKey);
			}
        }
	}

}
