using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class InputManager : Node2D
{
	public static bool IsPaused = false;
	public static TargetedAbilityInfo AbilityInfo { get; private set; } = null;
	[Export] public Color DragRectColor { get; set; }

	private bool isMouseDragging = false;

	[Export] private int mouseClickMoveTolerance = 4;

	private Rect2 dragRectBounds = new Rect2();
	private RectangleShape2D dragRectSelect = new RectangleShape2D();
	private Physics2DDirectSpaceState worldSpace;
	private Physics2DShapeQueryParameters query;
	private Vector2 mousePressOrigin;

	private Timer inputDelayTimer;
	[Export] private float inputDelayTimerLength = 0.005f; //Time to wait btwn inputs
	bool inputDelayActive = true;

	public override void _Ready()
	{
		//Setup
		worldSpace = GetWorld2d().DirectSpaceState;
		query = new Physics2DShapeQueryParameters();
		inputDelayTimer = (Timer)GetNode("InputDelayTimer");

		inputDelayTimer.Start();
	}

	//TODO: Break apart into smaller methods
	public override void _UnhandledInput(InputEvent @event)
	{
		float delta = GetPhysicsProcessDeltaTime();

		if (!inputDelayActive)
		{
			if (AbilityInfo != null)
			{
				HandleTargetInput();
			}
			else //Standard input
			{
				HandleNonTargetInput();
			}
		}

		HandleCameraInput(delta);

	}

	private void HandleTargetInput()
	{		
		if (Input.IsActionJustReleased("left_click")) //attempt to fire
		{
			//THIS ORDERING IS IMPORTANT! Set active right away
			inputDelayActive = true;

			var clickable = GetMostClickableAt(GetGlobalMousePosition());
			if (AbilityInfo.IsValid && AbilityInfo.Ability.SetTarget(clickable))
			{
				AbilityInfo.Caster.QueuedAbility = AbilityInfo.Ability;
			}

			GetTree().SetInputAsHandled();
			SetTargetedAbility(null);

			inputDelayTimer.Start(inputDelayTimerLength);
		}
		else if (Input.IsActionJustPressed("right_click")) //cancel out
		{
			SetTargetedAbility(null);
		}

		
	}

	private void HandleNonTargetInput()
	{
		if (inputDelayActive)
			return;

		var mouseLocation = GetGlobalMousePosition();
		ClickInfo clickInfo = new ClickInfo();

		if (Input.IsActionJustPressed("left_click")) //Initial click
			mousePressOrigin = mouseLocation;

		if (Input.IsActionPressed("left_click") && !isMouseDragging)
		{
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
			IMapClickable clickTarget = GetMostClickableAt(mouseLocation);
			
			clickInfo.ButtonNumber = 1;
			clickInfo.ModifyHeld = (Input.IsActionPressed("modify"));

			if (clickTarget != null)
				clickTarget.ClickAction(clickInfo, mouseLocation);

		}
		else if (Input.IsActionJustReleased("left_click") && isMouseDragging)
			isMouseDragging = false;
	}

	private void HandleCameraInput(float delta)
	{
		CameraControls.ProcessInput(delta);
	}

	public override void _Draw()
	{
		/// TODO separate the mechanics out of the drawing
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

			LocalCharacterManager.SelectAllInRect(players, !Input.IsActionPressed("modify"));
		}
	}

	private void HandleMouseDrag()
	{
		Update();
	}

	public IMapClickable GetMostClickableAt(Vector2 position)
	{
		var collisions = worldSpace.IntersectPoint(position, 32, null, 2147483647, true, true);
		return GetMostClickable(collisions);
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

	/// <summary>
	/// Queue user's ability to fire. Requires that the used ability is passed in; this
	/// gives this mgr access to the valid ability range 
	/// </summary>
	/// <param name="ability"></param>
	public static void SetTargetedAbility(TargetedAbilityInfo taInfo)
	{
		AbilityInfo = taInfo;

		if (AbilityInfo != null)
			Input.SetDefaultCursorShape(Input.CursorShape.Cross);
		else
			Input.SetDefaultCursorShape(); //default
	}

	/// <summary>
	/// Syntactic sugar. Will make it clearer when other classes interrupt the ability.
	/// </summary>
	public static void InterruptTargetedAbility()
	{
		SetTargetedAbility(null);
	}

	/// <summary>
	/// Connected by signal from child timer node.
	/// </summary>
	private void InputDelayTimeout()
	{
		inputDelayActive = false;
	}
}
