using UnityEngine;

public class CharacterFallState : CharacterAbstractState
{
    public CharacterFallState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubStates();

        PlayerContextManager.GroundChecker.enabled = true;

        PlayerContextManager.Rigidbody.gravityScale = 4.71f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void LateUpdateState()
    {
        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.FALL_ANIMATION);
    }
    public override void ExitState()
    {
        
    }
    public override void CheckSwitchStates()
    {
        
    }
    public override void InitializeSubStates()
    {
        if (PlayerContextManager.MoveInput != 0)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
        else if (PlayerContextManager.MoveInput == 0)
        {
            SetSubState(PlayerStateFactory.IdleState());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        SwitchState(PlayerStateFactory.GroundedState());
    }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
