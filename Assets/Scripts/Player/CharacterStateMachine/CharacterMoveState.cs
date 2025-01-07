using UnityEngine;

public class CharacterMoveState : CharacterAbstractState
{
    public CharacterMoveState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
       
    }
    public override void UpdateState()
    {
        PlayerContextManager.Rigidbody.linearVelocity = new Vector2(PlayerContextManager.MoveInput * 5f, PlayerContextManager.VerticalVelocity);

        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        
    }
    
    public override void LateUpdateState()
    {
        if (PlayerContextManager.CurrentState == PlayerStateFactory.GroundedState() || PlayerContextManager.CurrentState == PlayerStateFactory.InteractionState())
        {
            
            PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.RUN_ANIMATION);
        }
    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.MoveInput == 0 || PlayerContextManager.IsWallColliding)
        {
            SwitchState(PlayerStateFactory.IdleState());
        }
    }
    public override void InitializeSubStates()
    {
        
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
