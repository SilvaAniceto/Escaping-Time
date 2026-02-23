using UnityEngine;

public class CharacterOnWallState : CharacterAbstractState
{
    public CharacterOnWallState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.WallChecker.enabled = true;

        CharacterContextManager.PlayerInputManager.ClearAirJumpCommandCombo();

        if (CharacterContextManager.HasAirJump)
        {
            CharacterContextManager.AirJumpIsAllowed = true;
        }

        CharacterAnimationManager.CharacterAnimator.transform.rotation *= Quaternion.Euler(0, 180, 0);

        CharacterContextManager.DashIsWaitingGroundedState = false;

        System.Action action = () =>
        {
            CharacterContextManager.DashOnCoolDown = false;
        };

        CharacterContextManager.WaitFrameEnd(action);
    }

    public override void UpdateState()
    {
        CharacterContextManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.JumpSpeed = Mathf.Lerp(0.00f, -3.00f, CharacterContextManager.GetFallSpeedLerpOvertime());
    }

    public override void FixedUpdateState()
    {

    }

    public override void LateUpdateState()
    {
        CharacterAnimationManager.SetOnWallAnimation();
        GameAudioManager.Instance.PlayCharacterSFX("OnWall", 0.062f);
    }

    public override void ExitState()
    {
        CharacterContextManager.JumpSpeed = 0.00f;
        CharacterContextManager.FallStartSpeed = CharacterContextManager.JumpSpeed;
    }

    public override void CheckSwitchStates()
    {
        if (Grounded)
        {
            SwitchState(CharacterStateFactory.GroundedState());
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

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }    

    public override void OnTriggerExit2D(Collider2D collision) 
    {
        SwitchState(CharacterStateFactory.FallState());
    }
}
