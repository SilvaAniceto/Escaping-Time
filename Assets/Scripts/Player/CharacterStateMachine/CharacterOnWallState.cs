using UnityEngine;

public class CharacterOnWallState : CharacterAbstractState
{
    public CharacterOnWallState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (CharacterContextManager.HasTemporaryWallMove && CharacterContextManager.TemporaryWallMoveTime == 0.00f)
        {
            CharacterContextManager.ResetTemporaryWallMoveTime(6.00f);
        }

        CharacterContextManager.WallChecker.enabled = true;

        if (CharacterContextManager.HasAirJump)
        {
            CharacterContextManager.AirJumpIsAllowed = true;
        }

        CharacterAnimationManager.CharacterAnimator.transform.rotation *= Quaternion.Euler(0, 180, 0);

        CharacterContextManager.ResetDashCoolDownTime(0);
    }

    public override void UpdateState()
    {
        CharacterContextManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.VerticalSpeed = Mathf.Lerp(0.00f, -3.00f, CharacterContextManager.GravityDownwardSpeedLerpOvertime);
    }

    public override void FixedUpdateState()
    {

    }

    public override void LateUpdateState()
    {
        CharacterAnimationManager.SetOnWallAnimation();
    }

    public override void ExitState()
    {
        CharacterContextManager.VerticalSpeed = 0.00f;
        CharacterContextManager.FallStartSpeed = CharacterContextManager.VerticalSpeed;
    }

    public override void CheckSwitchStates()
    {
        if (Grounded)
        {
            SwitchState(CharacterStateFactory.GroundedState());
        }

        if (PlayerInputManager.DashInput && CharacterContextManager.DashIsAllowed)
        {
            SwitchState(CharacterStateFactory.DashState());
        }

        if (PlayerInputManager.StartJumpInput)
        {
            SwitchState(CharacterStateFactory.WallJumpState());
        }

        if (!PlayerInputManager.WallMoveInput)
        {
            SwitchState(CharacterStateFactory.FallState());
        }
    }

    public override void CheckSwitchSubStates()
    {
        
    }

    public override Quaternion CurrentLookRotation()
    {
        float angle = Mathf.Atan2(0, PlayerInputManager.MoveInput) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.up);
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }    

    public override void OnTriggerExit2D(Collider2D collision) 
    {
        //if (collision.CompareTag("Ceiling") || collision.CompareTag("Interactable"))
        //{
        //}
            SwitchState(CharacterStateFactory.FallState());
    }
}
