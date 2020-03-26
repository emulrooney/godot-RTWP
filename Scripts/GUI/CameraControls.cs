using Godot;
using System;

public class CameraControls : Node2D
{

    private CameraControls _cc;
    private Camera2D _camera;
    private ColorRect _select;

    private Vector2 cameraMove;
    private bool isCameraLocked = false;

    private Vector2 mousePressOrigin;
    private bool isMousePressed = false;

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
        _select = (ColorRect)GetNode("Camera2D/ColorRect");

    }

    public void ProcessInput(float delta)
    {
        //Set rather than += to ensure start from 0
        cameraMove = GetKeyboardInput() * CameraKeySpeed;

        if (MouseMoveEnabled)
        {
            //This can be += in case someone is trying to go superduper fast
            cameraMove += GetMouseInput() * CameraMouseSpeed;
        }

        //Get click action
        HandleDrag();
    }

    private void HandleDrag()
    {
        if (Input.IsMouseButtonPressed(1))
        {
            if (!isMousePressed)
            {
                isMousePressed = true;
                _select.Visible = true;
                mousePressOrigin = GetGlobalMousePosition();
                _select.SetGlobalPosition(mousePressOrigin);
            }

            var size = GetGlobalMousePosition() - mousePressOrigin;
            _select.SetSize(new Vector2(Math.Abs(size.x), Math.Abs(size.y)));

            int flipX, flipY = 0;
            if (mousePressOrigin.x <= GetGlobalMousePosition().x)
                flipX = 1;
            else
                flipX = -1;
            if (mousePressOrigin.y <= GetGlobalMousePosition().y)
                flipY = 1;
            else
                flipY = -1;

            _select.SetScale(new Vector2(flipX, flipY));
        }
        else if (isMousePressed)
        {
            isMousePressed = false;
            //MapCharacterManager.SelectAllInRect(_selectArea.GetOverlappingBodies());
            //Select all players
            _select.Visible = false;
        }
    }

    public Vector2 GetKeyboardInput()
    {
        int x = 0, y = 0;

        if (Input.IsActionPressed("ui_up"))
            y--;

        if (Input.IsActionPressed("ui_down"))
            y++;

        if (Input.IsActionPressed("ui_left"))
            x--;

        if (Input.IsActionPressed("ui_right"))
            x++;

        if (x != 0 || y != 0)
            isCameraLocked = false;

        return new Vector2(x, y);
    }

    public Vector2 GetMouseInput()
    {
        int x = 0, y = 0;

        var screenRect = GetViewport().GetVisibleRect();
        var mousePos = GetLocalMousePosition() + (screenRect.Size / 2);

        if (screenRect.Size.x - mousePos.x <= MouseMoveThreshold)
            x++;
        if (mousePos.x >= MouseMoveThreshold)
            x--;
        if (screenRect.Size.y - mousePos.y <= MouseMoveThreshold)
            y++;
        if (mousePos.y >= MouseMoveThreshold)
            y--;

        if (x != 0 || y != 0)
            isCameraLocked = false;

        return new Vector2(x, y);
    }

    public void FocusPartyMember(int member)
    {
        CameraFocus = MapCharacterManager.SelectPartyMember(member);

        if (CameraFocus != null)
        {
            _camera.Position = CameraFocus.Position;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (CameraFocus != null && isCameraLocked)
        {
            _camera.Position = CameraFocus.Position;
        }

        _camera.Translate(cameraMove * delta);
    }

}
