using UnityEngine;

public class CharacterMoveState : CharacterAbstractState
{
    public CharacterMoveState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        CharacterContextManager.RemoveFixedJoint2D();
    }
    public override void UpdateState()
    {
        if (CharacterContextManager.CurrentState != CharacterStateFactory.OnWallState())
        {
            CharacterAnimationManager.CharacterAnimator.transform.rotation = CurrentLookRotation();
        }

        CharacterContextManager.HorizontalSpeed = PlayerInputManager.MoveInput * Mathf.Lerp(CharacterContextManager.HorizontalStartSpeed, 7.0f, CharacterContextManager.HorizontalSpeedLerpOvertime);

        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
                
    }
    
    public override void LateUpdateState()
    {
        if (CharacterContextManager.CurrentState == CharacterStateFactory.GroundedState() || CharacterContextManager.CurrentState == CharacterStateFactory.InteractionState())
        {
            CharacterAnimationManager.SetRunAnimation();
        }
    }
    public override void ExitState()
    {
        
    }
    public override void CheckSwitchStates()
    {
        if (PlayerInputManager.MoveInput == 0 || IsWallColliding)
        {
            SwitchState(CharacterStateFactory.IdleState());
        }
    }
    public override void CheckSwitchSubStates()
    {
        
    }
    public override Quaternion CurrentLookRotation()
    {
        float angle = Mathf.Atan2(0, PlayerInputManager.MoveInput) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.up);
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
