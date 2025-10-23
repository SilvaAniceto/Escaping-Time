using UnityEngine;

public class CharacterDashState : CharacterAbstractState
{
    public CharacterDashState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.DisableFixedJoint2D();

        CharacterContextManager.DashSpeedOvertime = 0;

        if (CharacterContextManager.ExitState == CharacterStateFactory.JumpState() || CharacterContextManager.ExitState == CharacterStateFactory.FallState())
        {
            CharacterContextManager.ExitState = CharacterStateFactory.FallState();
            CharacterContextManager.DashIsWaitingGroundedState = true;
        }

        CharacterContextManager.DashOnCoolDown = true;

        GameAudioManager.Instance.StopCharacterSFX();
        GameAudioManager.Instance.PlayCharacterSFX("Dash");
    }
    public override void UpdateState()
    {
        DashSpeed = Mathf.Lerp(25.0f, 0.0f, CharacterContextManager.DashSpeedLerpOvertime);

        CharacterContextManager.HorizontalSpeed = DashSpeed * CharacterForwardDirection;
        CharacterContextManager.VerticalSpeed = 0.00f;
    }
    public override void FixedUpdateState()
    {

    }
    public override void LateUpdateState()
    {
        CharacterAnimationManager.SetDashAnimation();
    }
    public override void ExitState()
    {
        if (CharacterContextManager.HasTemporaryDash)
        {
            CharacterContextManager.HasTemporaryDash = false;
            CharacterContextManager.DispatchPowerUpInteractableRecharge();
        }
        CharacterContextManager.HorizontalSpeed = 0.00f;
    }
    public override void CheckSwitchStates()
    {
        if (Mathf.Abs(CharacterContextManager.HorizontalSpeed) <= 7.0f)
        {
            if (CharacterContextManager.ExitState == CharacterStateFactory.OnWallState())
            {
                SwitchState(CharacterStateFactory.FallState());
            }
            else
            {
                SwitchState(CharacterContextManager.ExitState);
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

    public override void OnTriggerEnter2D(Collider2D collision) 
    {
        CharacterContextManager.ExitState = CharacterStateFactory.GroundedState();
    }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) 
    {
        CharacterContextManager.DashIsWaitingGroundedState = true;
        CharacterContextManager.ExitState = CharacterStateFactory.FallState();
    }
}
