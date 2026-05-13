using UnityEngine;

public class CharacterGroundedState : CharacterAbstractState
{
    public CharacterGroundedState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (CharacterContextManager.Rigidbody.bodyType == RigidbodyType2D.Kinematic)
        {
            CharacterContextManager.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        CharacterContextManager.PlayerInputManager.ClearAirJumpCommandCombo();

        CharacterContextManager.EnableFixedJoint2D();

        CharacterContextManager.Rigidbody.gravityScale = 50.00f;

        CharacterContextManager.PhysicsManager.HorizontalTopSpeed = 6.30f;

        CharacterContextManager.CeilingChecker.enabled = true;

        CharacterContextManager.WallChecker.enabled = false;

        if (CharacterContextManager.PowerUpManager.HasAirJump)
        {
            CharacterContextManager.PowerUpManager.AirJumpIsAllowed = true;
        }

        CharacterContextManager.PowerUpManager.DashIsWaitingGroundedState = false;

        System.Action dashAction = () =>
        {
            CharacterContextManager.PowerUpManager.DashOnCoolDown = false;
        };

        CharacterContextManager.WaitSeconds(dashAction, 0.25f);

        CharacterContextManager.PhysicsManager.HorizontalStartSpeed = CharacterContextManager.PhysicsManager.HorizontalSpeed;

        if (CharacterContextManager.DamageManager.IsInvincible)
        {
            CharacterContextManager.PhysicsManager.HorizontalStartSpeed = 0.00f;
            CharacterContextManager.PhysicsManager.HorizontalSpeed = 0.00f;
            CharacterContextManager.PhysicsManager.JumpSpeed = 0.00f;

            System.Action damagedAction = () =>
            {
                CharacterContextManager.DamageManager.IsInvincible = false;
            };

            CharacterContextManager.WaitSeconds(damagedAction, 0.66f);
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
        CharacterContextManager.PhysicsManager.FallStartSpeed = 0.00f;

        if (CharacterContextManager.PowerUpManager.HasAirJump)
        {
            CharacterContextManager.PowerUpManager.AirJumpIsAllowed = true;
        }

        SetSubState(null);
    }

    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.DamageManager.IsInvincible) return;

        if (!Grounded)
        {
            CharacterContextManager.PhysicsManager.CoyoteTime = true;

            System.Action action = () =>
            {
                CharacterContextManager.PhysicsManager.CoyoteTime = false;
            };

            SwitchState(CharacterStateFactory.FallState());

            CharacterContextManager.WaitSeconds(action, 0.084f);
        }
    }

    public override void CheckSwitchSubStates()
    {
        if (CharacterContextManager.DamageManager.IsInvincible) return;
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
