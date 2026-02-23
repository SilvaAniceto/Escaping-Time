using UnityEngine;

public class CharacterJumpState : CharacterAbstractState
{
    public CharacterJumpState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.HorizontalStartSpeed = CharacterContextManager.HorizontalSpeed * CharacterContextManager.MoveDirection;
        CharacterContextManager.HorizontalTopSpeed = 7.86f;

        CharacterContextManager.DisableFixedJoint2D();

        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.JumpSpeedOvertime = 0;

        if (CharacterContextManager.HasAirJump)
        {
            CharacterContextManager.AirJumpIsAllowed = true;
        }

        GameAudioManager.Instance.StopCharacterSFX();
        GameAudioManager.Instance.PlayCharacterSFX("Jump");
    }
    public override void UpdateState()
    {
        CharacterContextManager.JumpSpeed = Mathf.Lerp(0.00f, 12.00f, CharacterContextManager.GetJumpSpeedLerpOvertime());
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
        if (CharacterContextManager.JumpSpeed <= 0.00f)
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
