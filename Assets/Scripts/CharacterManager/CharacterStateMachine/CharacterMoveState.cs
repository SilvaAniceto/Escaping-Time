using UnityEngine;

public class CharacterMoveState : CharacterAbstractState
{
    public CharacterMoveState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        CharacterContextManager.DisableFixedJoint2D();
    }
    public override void UpdateState()
    {
        if (CharacterContextManager.CurrentState == CharacterStateFactory.GroundedState() || CharacterContextManager.CurrentState == CharacterStateFactory.InteractionState())
        {
            GameContextManager.Instance.AudioManager.PlayCharacterSFX("Walk", 0.192f);
        }

        if (CharacterContextManager.CurrentState != CharacterStateFactory.OnWallState())
        {
            CharacterAnimationManager.CharacterAnimator.transform.rotation = CurrentLookRotation();
        }

        CharacterContextManager.PhysicsManager.HorizontalSpeed = CharacterContextManager.PhysicsManager.MoveDirection * Mathf.Lerp(CharacterContextManager.PhysicsManager.HorizontalStartSpeed, CharacterContextManager.PhysicsManager.HorizontalTopSpeed, CharacterContextManager.PhysicsManager.GetHorizontalSpeedLerpOvertime(Time.deltaTime));
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
        if (IsWallColliding)
        {
            SwitchState(CharacterStateFactory.IdleState());
        }
    }
    public override void CheckSwitchSubStates()
    {
        
    }
    public override Quaternion CurrentLookRotation()
    {
        float angle = Mathf.Atan2(0, CharacterContextManager.PhysicsManager.MoveDirection) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.up);
    }
}
