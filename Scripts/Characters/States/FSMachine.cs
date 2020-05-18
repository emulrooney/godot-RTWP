using Godot;
using System;
using System.Collections.Generic;

public class FSMachine : Node
{
	Character _owner { get; set; }
	Statblock _stats { get; set; }
	Tween _moveTween { get; set; }

	CharacterAnimator Animator { get; set; }
	Dictionary<FSMState, CharacterState> States { get; set; } = new Dictionary<FSMState, CharacterState>();
	public CharacterState Current { get; private set; }
	CharacterState Previous { get; set; }

	public override void _Ready()
	{
		foreach (var n in this.GetChildren())
		{
			var state = (CharacterState)n;
			States.Add(state.StateType, state);
		}

		Current = States[FSMState.Idle];
		Previous = States[FSMState.Idle];
	}

	public void Activate()
	{
		_owner = (Character)Owner;
		_stats = _owner.Stats;
		_moveTween = (Tween)_owner.GetNode("MoveTween");
		Animator = (CharacterAnimator)_owner.GetNode("Animator");
	}

	public override void _Process(float delta)
	{
		if (_owner.IsDead)
		{
			if (Current.StateType != FSMState.Dead)
				Transition(FSMState.Dead);

			return;
		}

		if (_owner.QueuedAbility != null) //HAS QUEUED AN ABILITY...
		{
			ProcessAbilityState(delta);
		}
		else if (_owner.AttackTarget == null) //NON-COMBAT MODE
		{
			ProcessNormalState(delta);
		}
		else //COMBAT MODE
		{
			ProcessCombatState(delta);
		}
	}

	private void ProcessNormalState(float delta)
	{
		switch (Current.StateType)
		{
			case FSMState.Idle:
				if (_owner.QueuedMoves.Count > 0)
					Transition(FSMState.Move);
				break;
			case FSMState.Move:
				if (_owner.QueuedMoves.Count > 0 || _moveTween.IsActive())
					MoveTowardsNextLocation();
				else
					Transition(FSMState.Idle);
				break;
			case FSMState.MoveAttack:
			default:
				Transition(FSMState.Idle);
				break;
		}
	}

	private void ProcessCombatState(float delta)
	{
		switch (Current.StateType)
		{
			case FSMState.Idle:

				//If you can attack, attack
				//If you can't reach them, move attack to them
				//If you can't attack but you can reach them, stay idle.

				if (_owner.CanAttackTarget(_owner.AttackTarget))
					Transition(FSMState.Attack);
				else if (!_owner.CanReachTarget(_owner.AttackTarget))
					Transition(FSMState.MoveAttack);
				break;
			case FSMState.MoveAttack:

				//Check if they're already dead; if so, end combat mode
				//Check if you're able to attack; if so, attack
				//Check if you're UNABLE to reach them; if so, move towards them
				//If you can't attack and you can reach them, just idle.

				if (_owner.AttackTarget.IsDead || _owner.AttackTarget.IsQueuedForDeletion()) //Stop attacking already dead enemies
					_owner.AttackTarget = null;
				else if (_owner.CanAttackTarget(_owner.AttackTarget))
					Transition(FSMState.Attack);
				else if (!_owner.CanReachTarget(_owner.AttackTarget))
					MoveTowards(_owner.AttackTarget.Position);
				else
					Transition(FSMState.Idle);
				break;
			case FSMState.Attack:

				//If animation is done, attack

				if (((TimedState)Current).Done)
				{
					_owner.Attack(_owner.AttackTarget);
					Transition(FSMState.Idle);
				}
				break;
			case FSMState.Move:
			default:

				//This covers any case of unexpected states when transitioning to combat mode.
				Transition(FSMState.Idle);
				break;
		}
	}

	private void ProcessAbilityState(float delta)
	{
		switch (Current.StateType)
		{
			case FSMState.ChargingAbility:
				if (Current.Done)
				{
					Transition(FSMState.CastingAbility);
					_owner.QueuedAbility.ChargeComplete();
				}
				break;
			case FSMState.CastingAbility:
				if (Current.Done)
				{
					_owner.QueuedAbility.Use();
					_owner.QueuedAbility = null;
					Transition(FSMState.Idle);
				}
				break;
			default:
				AbilityTransition(_owner.QueuedAbility, _owner.QueuedAbility.ChargeTime, _owner.QueuedAbility.AnimationName);
				break;
		}
	}

	public virtual void SetAttackTarget(Character character)
	{
		_owner.AttackTarget = character;
		_owner.QueuedMoves.Clear();
	}

	/// <summary>
	/// Gets the next queued move (which is an array of points from pathfinding) and progresses through them
	/// Starts by checking that current path step is valid in array
	/// If not, moves to next queued move (or ends moving if no queued moves)
	/// If yes, sets nav location, does standard move twds it and slowly iterates thru steps in array 
	/// </summary>
	/// <param name="modifier"></param>
	protected void MoveTowardsNextLocation(float modifier = 1f)
	{
		if (_owner.QueuedMoves.Count > 0)
		{
			if (!_moveTween.IsActive())
			{
				MoveTowards(_owner.QueuedMoves.Dequeue());
			}
		}
	}


	protected void MoveTowards(Vector2 target, float modifier = 1f)
	{
		var gridTarget = GM.GetRealPosition(target);
		
		_moveTween.InterpolateProperty(_owner, "position", _owner.Position, gridTarget, .22f);
        _moveTween.SetActive(true);

		if (gridTarget.x > _owner.Position.x)
		{
			SetFlip(target);
		}
	}

	private void SetFlip(Vector2 target)
	{
		if (!Animator.FlipH && target.x < _owner.Position.x)
			Animator.SetFlipHWithOffset(true);
		else if (Animator.FlipH && target.x > _owner.Position.x)
			Animator.SetFlipHWithOffset(false);
	}
	
	protected virtual void MoveToAttack(Character target)
	{
		MoveTowards(target.Position);
	}

	public void Transition(FSMState newState)
	{
		if (States.ContainsKey(newState))
		{
			Previous.OnFinish();
			States[newState].OnStart();
			Previous = Current;
			Current = States[newState];

			Animator.Animation = Current.AnimationName; //this gives an error in debugger but non-breaking; leaving it in to make debugging easier later

			_Process(0); //Essentially, keep transitioning until none left. Slightly hacky, but solves weird visual glitch
		}
	}

	/// <summary>
	/// Override of normal transition. Forces animation to play for duration of state.
	/// </summary>
	/// <param name="newState"></param>
	/// <param name="overrideAnimation"></param>
	public void Transition (FSMState newState, string overrideAnimation)
	{
		Transition(newState);

		if (!String.IsNullOrWhiteSpace(overrideAnimation))
			Animator.Animation = overrideAnimation;
	}


	public void AbilityTransition(Ability ability, float abilityLength, string overrideAnimation)
	{
		if (ability == null) //defensive
			return;
		else if (ability.IsCharged)
			Transition(FSMState.ChargingAbility, overrideAnimation);
		else
			Transition(FSMState.CastingAbility, overrideAnimation);

		TimedState timedState = (TimedState)Current;
		timedState.OverrideStateLength(abilityLength);
	}



	public void OwnerQueueFree()
	{
		_owner.QueueFree();
	}

}
