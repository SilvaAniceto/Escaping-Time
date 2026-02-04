using UnityEngine;

public class CharacterIdleState : CharacterAbstractState
{
    public CharacterIdleState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        CharacterContextManager.EnableFixedJoint2D();

        CharacterContextManager.HorizontalSpeed = 0.00f;

        if (CharacterContextManager.CurrentState == CharacterStateFactory.GroundedState())
        {
            CharacterContextManager.HorizontalSpeedOvertime = 0;
        }
    }
    public override void UpdateState()
    {
        
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void LateUpdateState()
    {
        if (CharacterContextManager.CurrentState == CharacterStateFactory.GroundedState() || CharacterContextManager.CurrentState == CharacterStateFactory.InteractionState())
        {
            CharacterAnimationManager.SetIdleAnimation();
        }
    }
    public override void ExitState()
    {
        CharacterContextManager.HorizontalStartSpeed = 3.50f;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.MoveDirection != 0 && !IsWallColliding)
        {
            if (!CharacterContextManager.DamageOnCoolDown)
            {
                SwitchState(CharacterStateFactory.MoveState());
            }
        }
    }
    public override void CheckSwitchSubStates()
    {

    }
    public override Quaternion CurrentLookRotation()
    {
        float angle = Mathf.Atan2(0, -CharacterContextManager.MoveDirection) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.up);
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
