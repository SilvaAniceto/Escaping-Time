using UnityEngine;

public class CharacterDashState : CharacterAbstractState
{
    public CharacterDashState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.DisableFixedJoint2D();

        CharacterContextManager.DashSpeedOvertime = 0.00f;

        if (!Grounded)
        {
            CharacterContextManager.DashIsWaitingGroundedState = true;
        }

        CharacterContextManager.DashOnCoolDown = true;

        GameAudioManager.Instance.StopCharacterSFX();
        GameAudioManager.Instance.PlayCharacterSFX("Dash");
    }
    public override void UpdateState()
    {
        DashSpeed = Mathf.Lerp(25.0f, 0.0f, CharacterContextManager.GetDashSpeedLerpOvertime());

        CharacterContextManager.HorizontalSpeed = DashSpeed * CharacterForwardDirection;
        CharacterContextManager.JumpSpeed = 0.00f;
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
        CharacterContextManager.HorizontalSpeed = 0.00f;
    }
    public override void CheckSwitchStates()
    {
        if (Mathf.Abs(CharacterContextManager.HorizontalSpeed) <= 7.0f)
        {
            SwitchState(CharacterStateFactory.FallState());
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

    }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) 
    {

    }
}
