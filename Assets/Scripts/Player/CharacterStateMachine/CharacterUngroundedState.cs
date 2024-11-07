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
        PlayerContextManager.PerformingJump = false;
        PlayerContextManager.Falling = false;
    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.Rigidbody.velocity.y < 0 && !PlayerContextManager.Falling)
        {
            SetSubState(PlayerStateFactory.FallState());
        }

        ProccessJumpInput(PlayerContextManager.JumpInput);
    }
    public override void InitializeSubStates()
    {
        if (PlayerContextManager.PerformingJump)
        {
            SetSubState(PlayerStateFactory.JumpState());            
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public override void OnCollisionStay(Collision collision)
    {

    }

    public override void OnCollisionExit2D(Collision2D collision)
    {

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        SwitchState(PlayerStateFactory.GroundedState());
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {

    }

    public override void OnTriggerExit2D(Collider2D collision)
    {

    }

    protected override void ProccessJumpInput(bool actioninput)
    {
        if (actioninput && _coyoteTime > 0)
        {
            PlayerContextManager.PerformingJump = true;

            PlayerContextManager.Rigidbody.velocity = new Vector2(PlayerContextManager.Rigidbody.velocity.x, 0);

            SetSubState(PlayerStateFactory.JumpState());
        }
    }
}
