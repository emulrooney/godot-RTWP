using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class InputManager : Node2D
{
	public static TargetedAbilityInfo AbilityInfo { get; private set; } = null;
	
	private Timer inputDelayTimer;
	[Export] private float inputDelayTimerLength = 0.005f; //Time to wait btwn inputs
	bool inputDelayActive = true;

	public override void _Ready()
	{
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
			inputDelayActive = true;
             
            if (TurnManager.Active is PlayerCharacter)
            {



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

		if (Input.IsActionJustReleased("left_click")) //Mouse up, normal click
		{
			IMapClickable clickTarget = (IMapClickable)LocalCharacterManager.GetCharacterAt(mouseLocation);
			
			clickInfo.ButtonNumber = 1;
			clickInfo.ModifyHeld = (Input.IsActionPressed("modify"));

            if (clickTarget != null)
                clickTarget.ClickAction(clickInfo, mouseLocation);
            else if (GM.Map != null)
            {

                if (TurnManager.Active is PlayerCharacter)
                {
                    TopPrinter.Two = "Origin: " + TurnManager.Active.GridPosition + " moving to " + GM.GetGridPosition(mouseLocation);


                    ((PlayerCharacter)TurnManager.Active).MoveTo(mouseLocation);
                    TopPrinter.Four = "is pc";
                }
                else
                {
                    TopPrinter.Four = "is not PC";
                }
            }
            else
                GD.Print("No map loaded.");

		}
	}

	private void HandleCameraInput(float delta)
	{
		CameraControls.ProcessInput(delta);
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
