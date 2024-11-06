using UnityEngine;

public class CharacterUngroundedState : CharacterAbstractState
{
    public CharacterUngroundedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubStates();
    }
    public override void UpdateState()
    {
        PlayerContextManager.CoyoteTime -= Time.deltaTime;
        PlayerContextManager.CoyoteTime = Mathf.Clamp01(PlayerContextManager.CoyoteTime);

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
        if (PlayerContextManager.Rigidbody.velocity.y < 0)
        {
            SetSubState(PlayerStateFactory.FallState());
        }

        if (PlayerContextManager.JumpInput && PlayerContextManager.CoyoteTime > 0)
        {
            PlayerContextManager.Rigidbody.velocity = new Vector2(PlayerContextManager.Rigidbody.velocity.x, 0);

            PlayerContextManager.PerformingJump = true;

            SetSubState(PlayerStateFactory.JumpState());
        }
    }
    public override void InitializeSubStates()
    {
        SetSubState(PlayerStateFactory.JumpState());
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
}
