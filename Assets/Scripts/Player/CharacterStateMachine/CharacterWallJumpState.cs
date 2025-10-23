using UnityEngine;

public class CharacterWallJumpState : CharacterAbstractState
{
    public CharacterWallJumpState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.GravityUpwardSpeedOvertime = 0;
        CharacterContextManager.HorizontalSpeedOvertime = 0;

        GameAudioManager.Instance.StopCharacterSFX();
        GameAudioManager.Instance.PlayCharacterSFX("Jump");
    }
    public override void UpdateState()
    {
        CharacterContextManager.VerticalSpeed = Mathf.Lerp(0.00f, 12.00f, CharacterContextManager.GravityUpwardSpeedLerpOvertime);
        CharacterContextManager.HorizontalSpeed = Mathf.Lerp(3.5f, 15.0f, CharacterContextManager.HorizontalSpeedLerpOvertime) * CharacterForwardDirection;
    }
    public override void FixedUpdateState()
    {

    }
    public override void LateUpdateState()
    {
        CharacterAnimationManager.SetJumpAnimation();
    }
    public override void ExitState()
    {
        CharacterContextManager.VerticalSpeed = 0.00f;
        CharacterContextManager.FallStartSpeed = 1.00f;
        CharacterContextManager.HorizontalStartSpeed = 15.0f;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.VerticalSpeed <= 1.00f)
        {
            SwitchState(CharacterStateFactory.FallState());
        }

        if (Mathf.Abs(CharacterContextManager.HorizontalSpeed) >= 7.50f && PlayerInputManager.MoveInput != 0 && PlayerInputManager.MoveInput != CharacterForwardDirection)
        {
            SwitchState(CharacterStateFactory.FallState());
        }

        if (PlayerInputManager.DashInput && CharacterContextManager.DashIsAllowed)
        {
            SwitchState(CharacterStateFactory.DashState());
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

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ceiling"))
        {
            SwitchState(CharacterStateFactory.FallState());
        }
    }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}