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

        CharacterContextManager.PhysicsManager.ResetJumpOvertime();
        CharacterContextManager.PhysicsManager.ResetHorizontalOvertime();

        GameAudioManager.Instance.StopCharacterSFX();
        GameAudioManager.Instance.PlayCharacterSFX("Jump");
    }
    public override void UpdateState()
    {
        CharacterContextManager.PhysicsManager.JumpSpeed = Mathf.Lerp(0.00f, 12.00f, CharacterContextManager.PhysicsManager.GetJumpSpeedLerpOvertime(Time.deltaTime));

        if (Mathf.Abs(CharacterContextManager.PhysicsManager.HorizontalSpeed) >= CharacterContextManager.PhysicsManager.HorizontalTopSpeed && CharacterContextManager.PhysicsManager.MoveDirection != 0 && CharacterContextManager.PhysicsManager.MoveDirection != CharacterForwardDirection)
        {
            CharacterContextManager.PowerUpManager.DisableAirJump();
            CharacterContextManager.PhysicsManager.HorizontalSpeed = Mathf.Lerp(3.5f, 15.0f, CharacterContextManager.PhysicsManager.GetHorizontalSpeedLerpOvertime(Time.deltaTime)) * CharacterForwardDirection * CharacterContextManager.PhysicsManager.MoveDirection;
        }
        else
        {
            CharacterContextManager.PhysicsManager.HorizontalSpeed = Mathf.Lerp(3.5f, 15.0f, CharacterContextManager.PhysicsManager.GetHorizontalSpeedLerpOvertime(Time.deltaTime)) * CharacterForwardDirection;
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
        CharacterContextManager.PhysicsManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.PhysicsManager.JumpSpeed = 0.00f;
        CharacterContextManager.PhysicsManager.FallStartSpeed = 1.00f;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.PhysicsManager.JumpSpeed <= 1.00f)
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

        if (Mathf.Abs(CharacterContextManager.PhysicsManager.HorizontalSpeed) >= CharacterContextManager.PhysicsManager.HorizontalTopSpeed && CharacterContextManager.PhysicsManager.MoveDirection != 0 && CharacterContextManager.PhysicsManager.MoveDirection != CharacterForwardDirection)
        {
            angle = Mathf.Atan2(0, CharacterContextManager.PhysicsManager.MoveDirection) * Mathf.Rad2Deg;
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