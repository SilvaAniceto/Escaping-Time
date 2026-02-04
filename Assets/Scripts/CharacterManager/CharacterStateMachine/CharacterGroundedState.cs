using UnityEngine;

public class CharacterGroundedState : CharacterAbstractState
{
    public CharacterGroundedState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (CharacterContextManager.Rigidbody.bodyType == RigidbodyType2D.Kinematic)
        {
            CharacterContextManager.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        PlayerInputManager.PlayerInputActions.Enable();

        PlayerInputManager.ClearAirJumpCommandCombo();

        CharacterContextManager.EnableFixedJoint2D();

        CharacterContextManager.Rigidbody.gravityScale = 50.00f;

        CharacterContextManager.HorizontalTopSpeed = 6.30f;

        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.WallChecker.enabled = false;

        if (CharacterContextManager.HasAirJump)
        {
            CharacterContextManager.AirJumpIsAllowed = true;
        }

        CharacterContextManager.DashIsWaitingGroundedState = false;

        System.Action dashAction = () =>
        {
            CharacterContextManager.DashOnCoolDown = false;
        };

        CharacterContextManager.WaitSeconds(dashAction, 0.25f);

        CharacterContextManager.HorizontalStartSpeed = CharacterContextManager.HorizontalSpeed;

        if (CharacterContextManager.DamageOnCoolDown)
        {
            CharacterContextManager.HorizontalStartSpeed = 0.00f;
            CharacterContextManager.HorizontalSpeed = 0.00f;
            CharacterContextManager.VerticalSpeed = 0.00f;

            System.Action damagedAction = () =>
            {
                CharacterContextManager.DamageOnCoolDown = false;
            };

            CharacterContextManager.WaitSeconds(damagedAction, 0.66f);
        }

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
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void LateUpdateState()
    {

    }

    public override void ExitState()
    {
        CharacterContextManager.Rigidbody.gravityScale = 0.00f;
        CharacterContextManager.FallStartSpeed = 0.00f;
    }

    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.DamageOnCoolDown) return;

        if (!Grounded)
        {
            SwitchState(CharacterStateFactory.FallState());
        }
    }

    public override void CheckSwitchSubStates()
    {
        if (CharacterContextManager.DamageOnCoolDown) return;
    }
    public override Quaternion CurrentLookRotation()
    {
        return new Quaternion();
    }
    public override void OnCollisionEnter2D(Collision2D collision) {  }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) 
    {
        
    }

    public override void OnTriggerStay2D(Collider2D collision) 
    {
        if (!collision.CompareTag("Interactable"))
        {
            if (collision.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.Interactions.Contains(EInteractionType.Stay))
                {
                    CharacterContextManager.Interactable = interactable;

                    SwitchState(CharacterStateFactory.InteractionState());
                }
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D collision) 
    {

    }
}
