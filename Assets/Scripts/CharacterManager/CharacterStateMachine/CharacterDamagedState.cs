using UnityEngine;

public class CharacterDamagedState : CharacterAbstractState
{

    public CharacterDamagedState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.DamageManager.IsInvincible = true;

        CharacterContextManager.DisableFixedJoint2D();

        CharacterContextManager.Rigidbody.gravityScale = 0.00f;

        CharacterContextManager.PhysicsManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.PhysicsManager.JumpSpeed = 0.00f;

        CharacterContextManager.PhysicsManager.ResetDashOvertime();
        CharacterContextManager.PhysicsManager.ResetHorizontalOvertime();
        CharacterContextManager.PhysicsManager.ResetFallOvertime();
    }
    public override void UpdateState()
    {
        CharacterContextManager.PhysicsManager.HorizontalSpeed = Mathf.Lerp(CharacterContextManager.PhysicsManager.HorizontalTopSpeed, 0.00f, CharacterContextManager.PhysicsManager.GetDamageSpeedLerpOvertime(Time.deltaTime)) * CharacterContextManager.DamageManager.DamageHitDirection;
        CharacterContextManager.PhysicsManager.JumpSpeed = Mathf.Lerp(10.00f, -20.00f, CharacterContextManager.PhysicsManager.GetFallSpeedLerpOvertime(Time.deltaTime));
    }
    public override void FixedUpdateState()
    {

    }

    public override void LateUpdateState()
    {
        CharacterAnimationManager.SetHitAnimation();
    }
    public override void ExitState()
    {
        CharacterContextManager.PhysicsManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.PhysicsManager.ResetHorizontalOvertime();
        CharacterContextManager.PowerUpManager.DashIsWaitingGroundedState = true;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.PhysicsManager.JumpSpeed <= -10.00f)
        {
            SwitchState(CharacterStateFactory.ResetState());
        }
    }
    public override void CheckSwitchSubStates()
    {

    }

    public override Quaternion CurrentLookRotation()
    {
        return new Quaternion();
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
