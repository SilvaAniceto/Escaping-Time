using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterDamagedState : CharacterAbstractState
{

    public CharacterDamagedState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.RemoveFixedJoint2D();

        CharacterContextManager.TakingDamage = false;

        CharacterContextManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.VerticalSpeed = 0.00f;

        CharacterContextManager.DamageSpeedOvertime = 0;
        CharacterContextManager.HorizontalSpeedOvertime = 0;
        CharacterContextManager.GravityDownwardSpeedOvertime = 0;

        CharacterContextManager.ResetDamageExitWaitTime();
    }
    public override void UpdateState()
    {
        CharacterContextManager.HorizontalSpeed = Mathf.Lerp(3.50f, 7.00f, CharacterContextManager.DamageSpeedLerpOvertime) * CharacterContextManager.DamageHitDirection;
        CharacterContextManager.VerticalSpeed = Mathf.Lerp(5.00f, -12.00f, CharacterContextManager.GravityDownwardSpeedLerpOvertime);
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
        CharacterContextManager.VerticalSpeed = 0.00f;
        CharacterContextManager.HorizontalStartSpeed = 0.00f;
        CharacterContextManager.HorizontalSpeedOvertime = 0.00f;
        CharacterContextManager.DashIsWaitingGroundedState = true;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.VerticalSpeed <= -5.00f && Grounded)
        {
            CharacterContextManager.SetDamageExitWaitTime();

            if (CharacterContextManager.DamageExitWaitTime <= 0.00f)
            {
                SwitchState(CharacterStateFactory.GroundedState());
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
