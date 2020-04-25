using Godot;
using System;
using System.Collections.Generic;

public class GM : Node
{
	public static GM _gm;

	public static RandomNumberGenerator RNG = new RandomNumberGenerator();

	//MANAGERS
	PartyManager PM;

	//SETTINGS
	public static int MouseClickMoveTolerance { get; } = 4;
	public static bool IsPaused = false;


	public override void _Ready()
	{
		if (_gm == null)
			_gm = this;
		else
		{
			QueueFree();
			return;
		}

		PM = (PartyManager)GetNode("Party");
	}

	public static void AddPartyToMap(MapLogic newMap)
	{
		if (!_gm.PM.Loaded)
			_gm.PM.LoadPartyData(); //Ensure this only happens once

		var characters = _gm.PM.GenerateLocalPlayerCharacters();

		for (int i = 0; i < characters.Length; i++)
		{
			if (characters[i] != null)
			{
				newMap.AddChild(characters[i]);
				newMap.PlaceCharacter(characters[i]);
				GUIManager.UpdateFor(characters[i]);
			}
		}
	}

	public static void UpdatePartyData()
	{
		GD.Print("ONE");
		_gm.PM.UpdatePartyData();
	}

}
