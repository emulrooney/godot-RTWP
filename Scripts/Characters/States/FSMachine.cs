using Godot;
using System;
using System.Collections.Generic;

public class FSMachine : Node
{
    Character _owner { get; set; }
    Statblock _stats { get; set; }
    CharacterAnimator Animator { get; set; }
    Dictionary<CharState, CharacterState> States { get; set; } = new Dictionary<CharState, CharacterState>();

    CharacterState Current { get; set; }
    CharacterState Previous { get; set; }

    //Used in pathfinding only
    private Vector2 nextNavLocation;
    private int currentPathStep = 0;

    public override void _Ready()
    {
        _owner = (Character)Owner;
        _stats = (Statblock)Owner.GetNode("Statblock");
        Animator = (CharacterAnimator)Owner.GetNode("AnimatedSprite");

        foreach (var n in this.GetChildren())
        {
            var state = (CharacterState)n;
            States.Add(state.StateType, state);
        }

        Current = States[CharState.Idle];
        Previous = States[CharState.Idle];
    }

    public override void _Process(float delta)
    {
        
        if (_owner.AttackTarget == null) //NON-COMBAT MODE
        {
            switch (Current.StateType)
            {
                case CharState.Idle:
                    if (_owner.QueuedMoves.Count > 0)
                        Transition(CharState.Move);
                    break;
                case CharState.Move:
                    if (_owner.QueuedMoves.Count > 0)
                        MoveTowardsNextLocation();
                    else
                        Transition(CharState.Idle);
                    break;
                case CharState.MoveAttack:
                default:
                    Transition(CharState.Idle);
                    break;
            }
        }
        else //COMBAT MODE
        {
            switch (Current.StateType)
            {
                case CharState.Idle:
                    
                    //If you can attack, attack
                    //If you can't reach them, move attack to them
                    //If you can't attack but you can reach them, stay idle.

                    if (_owner.CanAttackTarget(_owner.AttackTarget))
                        Transition(CharState.Attack);
                    else if (!_owner.CanReachTarget(_owner.AttackTarget))
                        Transition(CharState.MoveAttack);
                    break;
                case CharState.MoveAttack:

                    //Check if they're already dead; if so, end combat mode
                    //Check if you're able to attack; if so, attack
                    //Check if you're UNABLE to reach them; if so, move towards them
                    //If you can't attack and you can reach them, just idle.

                    if (_owner.AttackTarget.IsQueuedForDeletion()) //Stop attacking already dead enemies
                        _owner.AttackTarget = null;
                    else if (_owner.CanAttackTarget(_owner.AttackTarget))
                        Transition(CharState.Attack);
                    else if (!_owner.CanReachTarget(_owner.AttackTarget))
                        MoveTowards(_owner.AttackTarget.Position);
                    else
                        Transition(CharState.Idle);
                    break;
                case CharState.Attack:

                    //If animation is done, attack

                    if (((AttackState)Current).Done)
                    {
                        _owner.Attack(_owner.AttackTarget);
                        Transition(CharState.Idle);
                    }
                    break;
                case CharState.Move:
                default:
                    
                    //This covers any case of unexpected states when transitioning to combat mode.
                    Transition(CharState.Idle);
                    break;
            }

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
        if (currentPathStep == _owner.QueuedMoves.Peek().Length)
        {
            currentPathStep = 0;
            _owner.QueuedMoves.Dequeue();
            return;
        }

        nextNavLocation = _owner.QueuedMoves.Peek()[currentPathStep];

        if ((_owner.QueuedMoves.Count > 0) && (nextNavLocation - _owner.Position).Length() > Character.TargetTolerance)
        {
            MoveTowards(nextNavLocation);
        }
        else if (currentPathStep < _owner.QueuedMoves.Peek().Length)
        {
            currentPathStep++;
        }

    }


    protected void MoveTowards(Vector2 target, float modifier = 1f)
    {
        if ((target - _owner.Position).Length() > Character.TargetTolerance)
        {
            Vector2 velocity = (target - _owner.Position).Normalized() * _stats.MoveSpeed * modifier;
            _owner.MoveAndSlide(velocity);
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

    public void Transition(CharState newState)
    {
        if (States.ContainsKey(newState))
        {
            Previous.OnFinish();
            States[newState].OnStart();
            Previous = Current;
            Current = States[newState];
            
            Animator.Animation = Current.AnimationName;

            _Process(0); //Essentially, keep transitioning until none left. Slightly hacky, but solves weird visual glitch
        }
    }

}

public enum CharState
{
    Idle,
    Move,
    MoveAttack,
    Attack
}