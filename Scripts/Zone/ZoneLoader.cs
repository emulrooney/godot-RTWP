using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Class to handle loading next map when transitioning btwn maps
/// 
/// 
/// </summary>
public class ZoneLoader : Node
{
	public static ZoneLoader _zl;
	private const int maxPlayerMapTransitionDistance = 256; //players must be near eachother to leave map

	private MapLogic CurrentMap { get; set;}

	public static Dictionary<string, string> zones = new Dictionary<string, string>()
	{
        { "WORLDMAP", "res://WorldMap/WorldMap.tscn" },
		{ "debugMap1", "res://Maps/Map.tscn" }
	};

	public override void _Ready()
	{
		//Singleton
		if (_zl == null)
        {
			_zl = this;
        }
		else
			QueueFree();
	}

    public static void GetCurrentMap(bool forceGet = false)
    {
        if (_zl != null && (forceGet || _zl.CurrentMap == null || _zl.CurrentMap.IsQueuedForDeletion()))
        {                
            _zl.CurrentMap = _zl.GetTree().Root.GetNodeOrNull<MapLogic>("Map");
            _zl.CurrentMap.Name = "CurrentMap";
            GD.Print($"Map found {_zl.CurrentMap != null}");
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
			var partyPositions = MapCharacterManager.GetPartyPositions();
			foreach (Vector2 pcp in partyPositions)
				if (pcp.Dot((Vector2)triggeredFrom) > maxPlayerMapTransitionDistance)
					return false;
		}                

		//Test map can actually be found and loaded
		if (!(zones.ContainsKey(mapKey) && ResourceLoader.Exists(zones[mapKey])))
			return false;


		GUIManager.WipePartyElements();
		MapCharacterManager.ResetAll();
		//We'll need another class to persist characters and recreate on the next map

		//Run a screen transition and wipe the current map

		//Reload the new map

		//Setup MCM, Party Icons as needed
		var loaded = ResourceLoader.Load(zones[mapKey]) as PackedScene;

		if (_zl.CurrentMap != null)
        {
            _zl.CurrentMap.QueueFree();
        }

        _zl.GetTree().ChangeSceneTo(loaded);
        //Map will attempt to set itself as current map

        return true;
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		//debug only
		//if (Input.IsActionJustPressed("ui_left"))
		//	GD.Print("Loaded:: " + LoadMap("debugMap1"));		
	}
       
}
