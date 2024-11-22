using UnityEngine;

public class CharacterIdleState : CharacterAbstractState
{
    public CharacterIdleState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        if (PlayerContextManager.CurrentState == PlayerStateFactory.GroundedState())
        {
            PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.IDLE_ANIMATION);
        }

        PlayerContextManager.Rigidbody.velocity = new Vector2(0, PlayerContextManager.Rigidbody.velocity.y);
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

    }
    public override void ExitState()
    {
    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.MoveInput != 0 && !PlayerContextManager.IsWallColliding)
        {
            SwitchState(PlayerStateFactory.MoveState());
        }
    }
    public override void InitializeSubStates()
    {

    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public override void OnCollisionStay(Collision2D collision)
    {

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
