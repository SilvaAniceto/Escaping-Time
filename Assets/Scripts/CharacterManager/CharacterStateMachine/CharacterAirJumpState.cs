using UnityEngine;

public class CharacterAirJumpState : CharacterAbstractState
{
    public CharacterAirJumpState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.GravityUpwardSpeedOvertime = 0;

        CharacterContextManager.AirJumpIsAllowed = false;

        CharacterContextManager.GameContextManager.GameAudioManager.StopCharacterSFX();
        CharacterContextManager.GameContextManager.GameAudioManager.PlayCharacterSFX("Air_Jump");

        if (CharacterContextManager.MoveDirection != 0)
        {
            SetSubState(CharacterStateFactory.MoveState());
        }
        else if (CharacterContextManager.MoveDirection == 0)
        {
            SetSubState(CharacterStateFactory.IdleState());
        }
    }
    public override void UpdateState()
    {
        CharacterContextManager.VerticalSpeed = Mathf.Lerp(0.00f, 12.00f, CharacterContextManager.GravityUpwardSpeedLerpOvertime);
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
        CharacterContextManager.FallStartSpeed = 0.00f;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.VerticalSpeed <= 0.20f)
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