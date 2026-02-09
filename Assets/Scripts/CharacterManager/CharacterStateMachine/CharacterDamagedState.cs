using UnityEngine;

public class CharacterDamagedState : CharacterAbstractState
{

    public CharacterDamagedState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.DamageOnCoolDown = true;

        CharacterContextManager.DisableFixedJoint2D();

        CharacterContextManager.Rigidbody.gravityScale = 0.00f;

        CharacterContextManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.VerticalSpeed = 0.00f;

        CharacterContextManager.DamageSpeedOvertime = 0;
        CharacterContextManager.HorizontalSpeedOvertime = 0;
        CharacterContextManager.GravityDownwardSpeedOvertime = 0;
    }
    public override void UpdateState()
    {
        CharacterContextManager.HorizontalSpeed = Mathf.Lerp(CharacterContextManager.HorizontalTopSpeed, 0.00f, CharacterContextManager.DamageSpeedLerpOvertime) * CharacterContextManager.DamageHitDirection;
        CharacterContextManager.VerticalSpeed = Mathf.Lerp(10.00f, -20.00f, CharacterContextManager.GravityDownwardSpeedLerpOvertime);
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
        CharacterContextManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.HorizontalSpeedOvertime = 0.00f;
        CharacterContextManager.DashIsWaitingGroundedState = true;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.VerticalSpeed <= -10.00f)
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
