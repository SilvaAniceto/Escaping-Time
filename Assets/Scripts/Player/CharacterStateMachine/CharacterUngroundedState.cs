using UnityEngine;

public class CharacterUngroundedState : CharacterAbstractState
{
    private float _coyoteTime;

    public CharacterUngroundedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.FALL_ANIMATION);

        _coyoteTime = 0.15f;

        InitializeSubStates();
    }
    public override void UpdateState()
    {
        _coyoteTime -= Time.deltaTime;
        _coyoteTime = Mathf.Clamp01(_coyoteTime);

        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void LateUpdateState()
    {

    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.JumpInput && _coyoteTime > 0)
        {
            SwitchState(PlayerStateFactory.JumpState());
        }
        else if (PlayerContextManager.Rigidbody.velocity.y < 0 && _coyoteTime <= 0)
        {
            SwitchState(PlayerStateFactory.FallState());
        }
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
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public override void OnCollisionStay(Collision2D collision)
    {
        SwitchState(PlayerStateFactory.GroundedState());
    }

    public override void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {

    }

    public override void OnTriggerExit2D(Collider2D collision)
    {

    }
}
