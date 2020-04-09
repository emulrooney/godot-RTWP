using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class InputManager : Node2D
{
	[Export] GameState Current { get; set; }
	[Export] public Color DragRectColor { get; set; }

	private bool isMouseDragging = false;

	[Export] private int mouseClickMoveTolerance = 4;

	private Rect2 dragRectBounds = new Rect2();
	private RectangleShape2D dragRectSelect = new RectangleShape2D();
	private Physics2DDirectSpaceState worldSpace;
	private Physics2DShapeQueryParameters query;
	private Vector2 mousePressOrigin;


	public override void _Ready()
	{
		//Setup
		worldSpace = GetWorld2d().DirectSpaceState;
		query = new Physics2DShapeQueryParameters();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//TODO --> JustReleased for player movement is SLOW. What's going on? 

		var mouseLocation = GetGlobalMousePosition();

		ClickInfo clickInfo = new ClickInfo();

		if (Input.IsActionJustPressed("left_click")) //Initial click
			mousePressOrigin = mouseLocation;

		if (Input.IsActionPressed("left_click") && !isMouseDragging) {
			if (mouseLocation.x > mousePressOrigin.x + mouseClickMoveTolerance
				|| mouseLocation.x < mousePressOrigin.x - mouseClickMoveTolerance
				|| mouseLocation.y > mousePressOrigin.y + mouseClickMoveTolerance
				|| mouseLocation.y < mousePressOrigin.y - mouseClickMoveTolerance)
			{
				isMouseDragging = true;
			}
		}

		if (isMouseDragging)
			HandleMouseDrag();

		if (Input.IsActionJustReleased("left_click") && !isMouseDragging) //Mouse up, normal click
		{
			//1. Check for collisions with characters, map
			var collisions = worldSpace.IntersectPoint(mouseLocation, 32, null, 2147483647, true, true);
			IMapClickable clickTarget;

			clickTarget = GetMostClickable(collisions);
			clickInfo.ButtonNumber = 1;
			clickInfo.ModifyHeld = (Input.IsActionPressed("modify"));

			if (clickTarget != null)
				clickTarget.ClickAction(clickInfo, mouseLocation);

		} else if (Input.IsActionJustReleased("left_click") && isMouseDragging)
			isMouseDragging = false;

	}

	/// <summary>
	/// TODO separate the mechanics out of the drawing
	/// </summary>
	public override void _Draw()
	{
		if (isMouseDragging)
		{
			dragRectBounds.Position = mousePressOrigin;
			dragRectBounds.Size = GetLocalMousePosition() - mousePressOrigin;
			DrawRect(dragRectBounds, DragRectColor, true);
		}
		else
		{
			query = new Physics2DShapeQueryParameters();
			dragRectSelect.Extents = (GetGlobalMousePosition() - mousePressOrigin) / 2;
			query.SetShape(dragRectSelect);
			query.Transform = new Transform2D(0, (GetGlobalMousePosition() + mousePressOrigin) / 2);

			var results = worldSpace.IntersectShape(query);
			List<PlayerCharacter> players = new List<PlayerCharacter>();

			foreach (Godot.Collections.Dictionary item in results)
			{
				if (item.Contains("collider"))
				{
					object collider = item["collider"];
					if (collider.GetType() == typeof(PlayerCharacter))
						players.Add((PlayerCharacter)collider);
				}
			}

			MapCharacterManager.SelectAllInRect(players, !Input.IsActionPressed("modify"));
		}
	}

	private void HandleMouseDrag()
	{
		Update();
	}


	/// <summary>
	/// Handles instances where multiple items found on click event.
	/// Players are immediately returned. If no players are found, the first found
	/// monster is returned. If no monsters and map logic was found, it's returned.
	/// Otherwise, null is returned.
	/// </summary>
	/// <param name="found">Array of items from intersect pt</param>
	/// <returns>"Most clickable" based on given rules as it's type</returns>
	public IMapClickable GetMostClickable(Array found)
	{
		if (found.Count == 0)
			return null;

		IMapClickable mostClickable = null;

		foreach (Dictionary item in found)
		{
			if (item.Contains("collider"))
			{
				switch (item["collider"])
				{
					case PlayerCharacter pc:
						return pc;
					case MonsterCharacter mc:
						if (mostClickable == null || mostClickable.GetType() != typeof(MonsterCharacter))
							mostClickable = mc;
						break;
					case MapLogic ml:
						if (mostClickable == null)
							mostClickable = ml;
						break;
					default:
						break;
				}
			}
		}

		return mostClickable;
	}
}
