using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class CameraControls : Node2D
{

	[Export] public Color DragRectColor { get; set; }

	private CameraControls _cc;
	private Camera2D _camera;

	//private Vector2 cameraMove;
	private bool isCameraLocked = false;

	public static Character CameraFocus { get; set; }

	//CONTROLS
	public bool MouseMoveEnabled { get; set; } = false;
	public float MouseMoveThreshold { get; set; } = 48; //Pixels from edge to start moving
	public float CameraKeySpeed { get; set; } = 300f;
	public float CameraMouseSpeed { get; set; } = 450f;


	public override void _Ready()
	{
		if (_cc == null)
			_cc = this;
		else
			QueueFree();

		_camera = (Camera2D)GetNode("Camera2D");

		GUIManager.RegisterElement(this);
	}

	/// <summary>
	/// Handles camera moving smoothly to follow focus
	/// </summary>
	/// <param name="delta"></param>
	public override void _PhysicsProcess(float delta)
	{
		if (CameraFocus != null && isCameraLocked)
		{
			_camera.Position = CameraFocus.Position;
		}

		//_camera.Translate(cameraMove * delta);
	}


	/// <summary>
	/// Public method to focus on party member
	/// TODO Might need a version for non-party members
	/// </summary>
	/// <param name="member"></param>
	public void FocusPartyMember(int member)
	{
		if (CameraFocus != null)
		{
			_camera.Position = CameraFocus.Position;
		}
	}

	///// <summary>
	///// Called by GUI Manager 
	///// </summary>
	///// <param name="delta"></param>
	//public void ProcessInput(float delta)
	//{
	//    //Set rather than += to ensure start from 0
	//    cameraMove = GetKeyboardInput() * CameraKeySpeed;

	//    if (MouseMoveEnabled)
	//    {
	//        //This can be += in case someone is trying to go superduper fast
	//        cameraMove += GetMouseInput() * CameraMouseSpeed;
	//    }
	//}

	/* INTERNAL */

	//private Vector2 GetKeyboardInput()
	//{
	//    int x = 0, y = 0;

	//    if (Input.IsActionPressed("ui_up"))
	//        y--;

	//    if (Input.IsActionPressed("ui_down"))
	//        y++;

	//    if (Input.IsActionPressed("ui_left"))
	//        x--;

	//    if (Input.IsActionPressed("ui_right"))
	//        x++;

	//    if (x != 0 || y != 0)
	//        isCameraLocked = false;

	//    return new Vector2(x, y);
	//}

	//private Vector2 GetMouseInput()
	//{
	//    int x = 0, y = 0;

	//    var screenRect = GetViewport().GetVisibleRect();
	//    var mousePos = GetLocalMousePosition() + (screenRect.Size / 2);

	//    if (screenRect.Size.x - mousePos.x <= MouseMoveThreshold)
	//        x++;
	//    if (mousePos.x >= MouseMoveThreshold)
	//        x--;
	//    if (screenRect.Size.y - mousePos.y <= MouseMoveThreshold)
	//        y++;
	//    if (mousePos.y >= MouseMoveThreshold)
	//        y--;

	//    if (x != 0 || y != 0)
	//        isCameraLocked = false;

	//    return new Vector2(x, y);
	//}
}


