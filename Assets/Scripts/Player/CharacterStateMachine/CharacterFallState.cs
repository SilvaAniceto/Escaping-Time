using UnityEngine;

public class CharacterFallState : CharacterAbstractState
{
    public CharacterFallState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.HorizontalTopSpeed = 6.30f;

        CharacterContextManager.CeilingChecker.enabled = false;
        CharacterContextManager.WallChecker.enabled = false;

        CharacterContextManager.GravityDownwardSpeedOvertime = 0;

        if (CharacterContextManager.ExitState == CharacterStateFactory.GroundedState())
        {
            CharacterContextManager.CoyoteTime = true;

            System.Action action = () =>
            {
                CharacterContextManager.CoyoteTime = false;
            };

            CharacterContextManager.WaitSeconds(action, 0.084f);
        }

        if (CharacterContextManager.HasAirJump && CharacterContextManager.ExitState == CharacterStateFactory.JumpState())
        {
            CharacterContextManager.AirJumpIsAllowed = true;
        }
    }
    public override void UpdateState()
    {
        CharacterContextManager.VerticalSpeed = Mathf.Lerp(CharacterContextManager.FallStartSpeed, -24.00f, CharacterContextManager.GravityDownwardSpeedLerpOvertime);
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void LateUpdateState()
    {
        if (!CharacterContextManager.DamageOnCoolDown)
        {
            CharacterAnimationManager.SetFallAnimation();
        }
    }
    public override void ExitState()
    {
        CharacterContextManager.VerticalSpeed = 0.00f;
    }
    public override void CheckSwitchStates()
    {
        if (Grounded)
        {
            SwitchState(CharacterStateFactory.GroundedState());
        }

        if (CharacterContextManager.DamageOnCoolDown) return;

        if (CharacterContextManager.HasWallMove)
        {
            if (IsWallColliding && PlayerInputManager.WallMoveInput)
            {
                SwitchState(CharacterStateFactory.OnWallState());
            }
        }

        if (PlayerInputManager.StartJumpInput && CharacterContextManager.CoyoteTime)
        {
            SwitchState(CharacterStateFactory.JumpState());
        }
        else if (CharacterContextManager.HasAirJump)
        {
            if (PlayerInputManager.StartJumpInput && CharacterContextManager.AirJumpIsAllowed)
            {
                SwitchState(CharacterStateFactory.AirJumpState()); 
            }
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
            if (PlayerInputManager.MoveInput != 0)
            {
                if (!CharacterContextManager.DamageOnCoolDown)
                {
                    SetSubState(CharacterStateFactory.MoveState());
                }
            }
            else if (PlayerInputManager.MoveInput == 0)
            {
                SetSubState(CharacterStateFactory.IdleState());
            }
        }
        else
        {
            if (PlayerInputManager.MoveInput != 0 && CurrentSubState == CharacterStateFactory.IdleState())
            {
                if (!CharacterContextManager.DamageOnCoolDown)
                {
                    SetSubState(CharacterStateFactory.MoveState());
                }
            }
            else if (PlayerInputManager.MoveInput == 0 && CurrentSubState == CharacterStateFactory.MoveState())
            {
                SetSubState(CharacterStateFactory.IdleState());
            }
        }
    }
    public override Quaternion CurrentLookRotation()
    {
        float angle = Mathf.Atan2(0, CharacterForwardDirection) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.up);
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
