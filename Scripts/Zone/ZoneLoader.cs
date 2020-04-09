using Godot;
using System;

/// <summary>
/// Class to handle loading next map when transitioning btwn maps
/// 
/// 
/// </summary>
public class ZoneLoader : Node
{
	public static ZoneLoader _zl;

	public void _Ready()
	{
		//Singleton
		if (_zl == null)
			_zl = this;
		else
			QueueFree();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="mapName"></param>
	public void LoadMap(string mapName)
	{
		//Test all party members in range of area transition
			//If no, give notice and wait
			//else continue

		//Test map can actually be found and loaded
			//If no, throw error
			//else, continue

		//Wipe party icons and any other GUI elements
		//Have MCM wipe all lists as needed
			//We'll need another class to persist characters and recreate on the next map

		//Run a screen transition and wipe the current map

		//Reload the new map

		//Setup MCM, Party Icons as needed
	}



}
