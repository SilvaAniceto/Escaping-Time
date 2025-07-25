using UnityEngine;

public class CharacterJumpState : CharacterAbstractState
{
    public CharacterJumpState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.HorizontalStartSpeed = CharacterContextManager.HorizontalSpeed * PlayerInputManager.MoveInput;
        CharacterContextManager.HorizontalTopSpeed = 7.86f;

        CharacterContextManager.RemoveFixedJoint2D();

        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.GravityUpwardSpeedOvertime = 0;

        if (CharacterContextManager.HasAirJump)
        {
            CharacterContextManager.AirJumpIsAllowed = true;
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
        if (CharacterContextManager.VerticalSpeed <= 0.00f || !PlayerInputManager.HoldJumpInput)
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
        if (CurrentSubState == null)
        {
            if (PlayerInputManager.MoveInput != 0 && !IsWallColliding)
            {
                SetSubState(CharacterStateFactory.MoveState());
            }
            else if (PlayerInputManager.MoveInput == 0)
            {
                SetSubState(CharacterStateFactory.IdleState());
            }
        }
        else
        {
            if (PlayerInputManager.MoveInput != 0 && CurrentSubState == CharacterStateFactory.IdleState() && !IsWallColliding)
            {
                SetSubState(CharacterStateFactory.MoveState());
            }
            else if (PlayerInputManager.MoveInput == 0 && CurrentSubState == CharacterStateFactory.MoveState())
            {
                SetSubState(CharacterStateFactory.IdleState());
            }
        }
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
