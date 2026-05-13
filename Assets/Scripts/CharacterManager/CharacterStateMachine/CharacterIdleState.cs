using UnityEngine;

public class CharacterIdleState : CharacterAbstractState
{
    public CharacterIdleState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        CharacterContextManager.EnableFixedJoint2D();

        CharacterContextManager.PhysicsManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.PhysicsManager.HorizontalStartSpeed = 0.00f;

        if (CharacterContextManager.CurrentState == CharacterStateFactory.GroundedState())
        {
            CharacterContextManager.PhysicsManager.ResetHorizontalOvertime();
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
        CharacterContextManager.PhysicsManager.HorizontalStartSpeed = 3.50f;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.PhysicsManager.MoveDirection != 0 && !IsWallColliding)
        {
            if (!CharacterContextManager.DamageManager.IsInvincible)
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
        return new Quaternion();
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
