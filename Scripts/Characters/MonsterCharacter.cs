using Godot;
using System;
using System.Collections.Generic;

public class MonsterCharacter : Character
{
    [Export] Vector2[] patrolPoints = new Vector2[0];
    public int PatrolCounter { get; set; } = 0;

    public override void _Ready()
    {
        base._Ready();

        //if (patrolPoints.Length > 0)
        //{
        //    Path.Enqueue(patrolPoints[0]);
        //    PatrolCounter++;
        //}
    }

    //public override void _PhysicsProcess(float delta)
    //{
    //    if (AttackTarget != null)
    //        MoveTowards(AttackTarget.Position);
    //    else if (Path.Count > 0)
    //        MoveTowardsNextLocation();
    //}

    public override void _Process(float delta)
    {
        if (patrolPoints.Length > 0 && PatrolCounter >= patrolPoints.Length)
            PatrolCounter = 0;

        //if (Path.Count == 0 && patrolPoints.Length > 0)
        //{
        //    Path.Enqueue(patrolPoints[PatrolCounter]);
        //    PatrolCounter++;
        //}
    }


    private void ClickCharacter(object viewport, object @event, int shape_idx)
    {
        MapCharacterManager.EnemyClicked(this);
    }

    private void CharacterEnteredAggro(object body)
    {
        //Signal from Aggro node
        try
        {
            var character = (Character)body;
            //SetAttackTarget(character);
        }
        catch (Exception e) 
        {
            GD.Print($"exception: {e}");
        }
    }

}


