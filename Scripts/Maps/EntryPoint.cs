using Godot;
using System;

/// <summary>
/// This is a temporary class for development; probably should be replaced with a
/// version that doesn't inherit from an area2D, but it's helpful for visualizing
/// in the editor. 
/// </summary>
public class EntryPoint : Area2D
{
	[Export] private Vector2[] spawnLocations;
	private bool[] occupied;

	public override void _Ready()
	{
		var map = (MapLogic)Owner;
		map.RegisterEntryPoint(this);

		occupied = new bool[spawnLocations.Length];
	}

	public Vector2 GetNextFreePoint()
	{
		for (int i = 0; i < spawnLocations.Length; i++)
		{
			if (!occupied[i])
			{
				occupied[i] = true;
				return spawnLocations[i];
			}
		}

		return Vector2.Zero;
	}


}
