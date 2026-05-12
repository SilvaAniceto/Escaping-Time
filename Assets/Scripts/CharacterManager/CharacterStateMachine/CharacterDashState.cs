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

        CharacterContextManager.PhysicsManager.ResetDashOvertime();

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
        DashSpeed = Mathf.Lerp(25.0f, 0.0f, CharacterContextManager.PhysicsManager.GetDashSpeedLerpOvertime(Time.deltaTime));

        CharacterContextManager.PhysicsManager.HorizontalSpeed = DashSpeed * CharacterForwardDirection;
        CharacterContextManager.PhysicsManager.JumpSpeed = 0.00f;
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
        CharacterContextManager.PhysicsManager.HorizontalSpeed = 0.00f;
    }
    public override void CheckSwitchStates()
    {
        if (Mathf.Abs(CharacterContextManager.PhysicsManager.HorizontalSpeed) <= 7.0f)
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
