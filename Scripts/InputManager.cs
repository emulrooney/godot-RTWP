using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class InputManager : Node2D
{
    [Export] GameState Current { get; set; }

    private Physics2DDirectSpaceState worldSpace;
    private Physics2DShapeQueryParameters query;

    public override void _Ready()
    {
        worldSpace = GetWorld2d().DirectSpaceState;
        
        query = new Physics2DShapeQueryParameters();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsMouseButtonPressed(1)) //TODO: Right clicks should be figured out
        {
            //Set up
            var mouseLocation = GetGlobalMousePosition();

            //1. Check for collisions with characters, map
            var collisions = worldSpace.IntersectPoint(mouseLocation, 32, null, 2147483647, true, true);
            IMapClickable clickTarget;

            clickTarget = GetMostClickable(collisions);

            if (clickTarget != null)
                clickTarget.ClickAction(mouseLocation);
        }
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
            if (item.ContainsKey("collider"))
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
