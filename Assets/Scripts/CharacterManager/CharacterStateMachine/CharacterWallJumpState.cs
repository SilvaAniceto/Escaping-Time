using UnityEngine;

public class CharacterWallJumpState : CharacterAbstractState
{
    public CharacterWallJumpState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory,CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.GravityUpwardSpeedOvertime = 0;
        CharacterContextManager.HorizontalSpeedOvertime = 0;

        CharacterContextManager.GameAudioManager.StopCharacterSFX();
        CharacterContextManager.GameAudioManager.PlayCharacterSFX("Jump");
    }
    public override void UpdateState()
    {
        CharacterContextManager.VerticalSpeed = Mathf.Lerp(0.00f, 12.00f, CharacterContextManager.GravityUpwardSpeedLerpOvertime);

        if (Mathf.Abs(CharacterContextManager.HorizontalSpeed) >= CharacterContextManager.HorizontalTopSpeed && CharacterContextManager.MoveDirection != 0 && CharacterContextManager.MoveDirection != CharacterForwardDirection)
        {
            CharacterContextManager.AirJumpIsAllowed = false;
            CharacterContextManager.HorizontalSpeed = Mathf.Lerp(3.5f, 15.0f, CharacterContextManager.HorizontalSpeedLerpOvertime) * CharacterForwardDirection * CharacterContextManager.MoveDirection;
        }
        else
        {
            CharacterContextManager.HorizontalSpeed = Mathf.Lerp(3.5f, 15.0f, CharacterContextManager.HorizontalSpeedLerpOvertime) * CharacterForwardDirection;
        }

        CharacterAnimationManager.CharacterAnimator.transform.rotation = CurrentLookRotation();
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
        CharacterContextManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.VerticalSpeed = 0.00f;
        CharacterContextManager.FallStartSpeed = 1.00f;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.VerticalSpeed <= 1.00f)
        {
            SwitchState(CharacterStateFactory.FallState());
        }
    }
    public override void CheckSwitchSubStates()
    {
       
    }

    public override Quaternion CurrentLookRotation()
    {
        float angle = 0;

        if (Mathf.Abs(CharacterContextManager.HorizontalSpeed) >= CharacterContextManager.HorizontalTopSpeed && CharacterContextManager.MoveDirection != 0 && CharacterContextManager.MoveDirection != CharacterForwardDirection)
        {
            angle = Mathf.Atan2(0, CharacterContextManager.MoveDirection) * Mathf.Rad2Deg;
        }
        else
        {
            angle = Mathf.Atan2(0, CharacterForwardDirection) * Mathf.Rad2Deg;
        }

        return Quaternion.AngleAxis(angle, Vector3.up);
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