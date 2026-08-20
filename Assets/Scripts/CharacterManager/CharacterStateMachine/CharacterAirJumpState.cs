using UnityEngine;

public class CharacterAirJumpState : CharacterAbstractState
{
    public CharacterAirJumpState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.PhysicsManager.ResetJumpOvertime();

        CharacterContextManager.PowerUpManager.DisableAirJump();

        ServiceLocator.AudioManager.StopCharacterSFX();
        ServiceLocator.AudioManager.PlayCharacterSFX("Air_Jump");
    }
    public override void UpdateState()
    {
        CharacterContextManager.PhysicsManager.JumpSpeed = Mathf.Lerp(0.00f, 12.00f, CharacterContextManager.PhysicsManager.GetJumpSpeedLerpOvertime(Time.deltaTime));
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
        CharacterContextManager.PhysicsManager.FallStartSpeed = 0.00f;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.PhysicsManager.JumpSpeed <= 0.20f)
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

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ceiling"))
        {
            SwitchState(CharacterStateFactory.FallState());
        }
    }
}