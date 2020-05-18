using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Class to handle loading next map when transitioning btwn maps
/// 
/// 
/// </summary>
public class MapLoader : Node
{
	public static MapLoader _ml;
	public static int EntryPoint { get; set; }
	private const int maxPlayerMapTransitionDistance = 256; //players must be near eachother to leave map

	public MapLogic CurrentMap { get; private set;}

	public static Dictionary<string, string> zones = new Dictionary<string, string>()
	{
		{ "WORLDMAP", "res://WorldMap/WorldMap.tscn" },
		{ "debugMap1", "res://Maps/Map.tscn" },
		
		{ "DEBUG_Fields1", "res://Maps/debug_Fields1.tscn" },
		{ "DEBUG_Fields2", "res://Maps/debug_Fields2.tscn" },
		{ "DEBUG_Cave", "res://Maps/debug_Cave.tscn" },

	};

	public override void _Ready()
	{
		//Singleton
		if (_ml == null)
		{
			_ml = this;
		}
		else
			QueueFree();
	}

	public static void GetCurrentMap(bool forceGet = false)
	{
		if (_ml != null && (forceGet || _ml.CurrentMap == null || _ml.CurrentMap.IsQueuedForDeletion()))
		{                
			_ml.CurrentMap = _ml.GetTree().Root.GetNodeOrNull<MapLogic>("Map");
			_ml.CurrentMap.Name = "CurrentMap";
			GD.Print($"Map found {_ml.CurrentMap != null}");
		} 
		else
			GD.Print("Already have map. Use 'forceGet' to overwrite");
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="mapName"></param>
	public static bool LoadMap(string mapKey, Vector2? triggeredFrom = null)
	{
		//Check distance is OK for all characters
		if (triggeredFrom != null)
		{
			var partyPositions = LocalCharacterManager.GetPartyPositions();
			foreach (Vector2 pcp in partyPositions)
				if (pcp.Dot((Vector2)triggeredFrom) > maxPlayerMapTransitionDistance)
					return false;
		}                

		//Test map can actually be found and loaded
		if (!(zones.ContainsKey(mapKey) && ResourceLoader.Exists(zones[mapKey])))
			return false;


		GUIManager.SetLoadables(false);
		GUIManager.WipePartyElements();
		LocalCharacterManager.ResetAll();
		//We'll need another class to persist characters and recreate on the next map

		//Run a screen transition and wipe the current map

		//Reload the new map

		//Setup MCM, Party Icons as needed
		var loaded = ResourceLoader.Load(zones[mapKey]) as PackedScene;

		if (_ml.CurrentMap != null)
		{
			_ml.CurrentMap.QueueFree();
		}

		_ml.GetTree().ChangeSceneTo(loaded);
		//Map will attempt to set itself as current map

		GUIManager.SetLoadables(true);
		return true;
	}

	public static void FocusCameraAtEntry()
	{
		try
		{
			//GUIManager.FocusAt(_ml.CurrentMap.EntryPoints[EntryPoint].Position);
		}
		catch (Exception e)
		{
			GD.Print(e);
			GD.Print("Invalid index for this map");
		}
	}
}
