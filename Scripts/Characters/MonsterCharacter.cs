using Godot;
using System;
using System.Collections.Generic;

public class MonsterCharacter : Character, IMapClickable
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
		//if (patrolPoints.Length > 0 && PatrolCounter >= patrolPoints.Length)
		//    PatrolCounter = 0;

		//if (Path.Count == 0 && patrolPoints.Length > 0)
		//{
		//    Path.Enqueue(patrolPoints[PatrolCounter]);
		//    PatrolCounter++;
		//}
	}


	public void ClickAction(ClickInfo info, Vector2 location)
	{
		//TODO Use location to determine which side of enemy to stand on (if needed)
		MapCharacterManager.EnemyClicked(this);
	}

}


