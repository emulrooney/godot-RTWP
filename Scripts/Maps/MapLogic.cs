using Godot;
using System;
using System.Collections.Generic;

public class MapLogic : Area2D, IMapClickable
{
	[Export] public List<EntryPoint> EntryPoints { get; private set; } = new List<EntryPoint>();

	private TileMap movementGrid;
	private TileMap nav;
	public Rect2 NavUsed { get; private set; }

	Godot.Collections.Array traversable;

	private AStar2D astar2D = new AStar2D();

	public override void _Ready()
	{
		movementGrid = (TileMap)GetNode("Movement");
		nav = (TileMap)GetNode("Nav");

		NavUsed = nav.GetUsedRect();

		MapLoader.GetCurrentMap(true);
		LocalCharacterManager.CurrentMap = this;
		GM.Map = this;
		GM.AddPartyToMap(this);

		GetTraversableTiles();
		ConnectTraversableTiles();
	}

	public void ClickAction(ClickInfo info, Vector2 location)
	{
		//ONLY USED FOR ABILITY TARGETING NOW    

		//GM.GetGridPosition(TurnManager.Active.Position);
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

	public bool IsFreeGridLocation(Vector2 location)
	{
		return nav.GetCell((int)location.x, (int)location.y) <= -1;
	}

	public Vector2[] GetPathInTiles(Vector2 from, Vector2 to)
	{
		var fromId = GetTileId(from);
		var toId = GetTileId(to);

		GD.Print("Checking " + fromId + " to " + toId);
		GD.Print($"{nav.GetCell((int)from.x, (int)from.y)} , {nav.GetCell((int)to.x, (int)to.y)}");

		if (!astar2D.HasPoint(fromId) || !astar2D.HasPoint(toId))
		{

			GD.Print($"BAD POINT {astar2D.HasPoint(fromId)} | {astar2D.HasPoint(toId)}");

			return null;
		}

		return astar2D.GetPointPath(fromId, toId);
	}


	//INTERNAL

	private int GetTileId(Vector2 mapLocation)
	{
		var x = mapLocation.x - NavUsed.Position.x;
		var y = mapLocation.y - NavUsed.Position.y;

		var id = (int)(x + y * NavUsed.Size.x);

		return id;
	}

	private void GetTraversableTiles()
	{
		traversable = nav.GetUsedCells(); //Have to use Godot's array

		foreach (Vector2 t in traversable)
		{
			astar2D.AddPoint(GetTileId(t), t);
		}
	}

	private void ConnectTraversableTiles()
	{
		foreach (Vector2 t in traversable)
		{
			var id = GetTileId(t);

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					var target = t + new Vector2(x, y);
					var targetId = GetTileId(target);

					if (t != target && astar2D.HasPoint(targetId))
					{
						astar2D.ConnectPoints(id, targetId, true);
					}
				}
			}

		}
	}
	
	public Vector2 GetGridLocation(Vector2 absolutePoint)
	{
		return nav.WorldToMap(absolutePoint);
	}

	public Vector2 GetRealPosition(Vector2 gridPoint)
	{
		return nav.MapToWorld(gridPoint);
	}

}
