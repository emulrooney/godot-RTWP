using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Minimum class to create enemy characters. 
/// TODO: Create abstract class for all NPC behaviours, have this inherit from the abstract one
/// TODO 2: This class can be the parent class to more involved behaviours
/// </summary>
public class EnemyBehaviour : Node
{
	private MonsterCharacter _owner;
	private FSMachine _fs;

	public List<Character> NearbyTargets { get; set; } = new List<Character>();

	public override void _Ready()
	{
		_fs = (FSMachine)Owner.GetNodeOrNull("FSMachine");

		if (_fs == null)
		{
			GD.Print($"{Owner.Name} does not have a state machine!");
			QueueFree();
		}

		_owner = (MonsterCharacter)Owner;
	}

	//Signal
	private void CharacterEnteredAggro(object body)
	{
		//Signal from Aggro node
		if (body != Owner)
		{
			Character character = (Character)body;

			if (!NearbyTargets.Contains(character))
				NearbyTargets.Add(character);

			if (_owner.AttackTarget == null)
				StartAttacking(character);
		}
	}

	//Signal
	private void CharacterExitedAggro(object body)
	{
		//Unset if this is the target
		if (_owner.AttackTarget == body)
			_owner.AttackTarget = null;

		//Remove from valid targets
		Character character;
		if (NearbyTargets.Contains(character = (Character)body))
			NearbyTargets.Remove(character);

		//TODO replace count check w distance check
		//Right now they'll attack people entering sequentially
		if (NearbyTargets.Count > 0)
			StartAttacking(NearbyTargets[0]);
	}

	private void StartAttacking(Character character)
	{
		try
		{
            GD.Print("attack: " + character.CharacterName);
			_owner.AttackTarget = character;
		}
		catch (Exception e)
		{
			GD.Print($"exception: {e}");
		}
	}
}


