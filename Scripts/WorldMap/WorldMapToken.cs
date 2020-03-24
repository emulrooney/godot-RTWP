using Godot;
using System;

public class WorldMapToken : KinematicBody2D
{
    [Export] public float TravelSpeed { get; set; } = 16f;
    [Export] public float TargetTolerance { get; set; }
    public Vector2 TargetLocation { get; protected set; }
    public bool HasTarget { get; set; } = false;
    [Export] public bool IsPlayer { get; set; }

    public override void _PhysicsProcess(float delta)
    {
        if (HasTarget)
            MoveTowardsLocation();
    }

    public void SetTarget(Vector2 location)
    {
        TargetLocation = location;
        HasTarget = true;
    }

    private void MoveTowardsLocation()
    {
        if ((TargetLocation - Position).Length() > TargetTolerance)
        {
            Vector2 velocity = (TargetLocation - Position).Normalized() * TravelSpeed;
            MoveAndSlide(velocity);
        }
        else
        {
            HasTarget = false;
        }
    }
}
