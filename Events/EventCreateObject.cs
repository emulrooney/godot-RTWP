using Godot;
using System;

public class EventCreateObject : Node
{
	[Export] public EventTrigger TriggerType { get; private set; }
	[Export] public PackedScene packedObject;
	[Export] public Vector2 location;

	public void Trigger()
	{
		Node2D newObject = (Node2D)packedObject.Instance();
		GetTree().Root.AddChild(newObject);
		newObject.Position = location;
	}
}

public enum EventTrigger
{
	ON_DEATH
}
