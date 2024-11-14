using UnityEngine;

public class CharacterUngroundedState : CharacterAbstractState
{
    private float _coyoteTime;

    public CharacterUngroundedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        _coyoteTime = 0.15f;

        InitializeSubStates();
    }
    public override void UpdateState()
    {
        _coyoteTime -= Time.deltaTime;
        _coyoteTime = Mathf.Clamp01(_coyoteTime);

        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void LateUpdateState()
    {

    }
    public override void ExitState()
    {
        PlayerContextManager.PerformingJump = false;
        PlayerContextManager.Falling = false;
    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.Damaged)
        {
            SwitchState(PlayerStateFactory.DamagedState());
        }

        if (PlayerContextManager.Rigidbody.velocity.y <= 0 && !PlayerContextManager.Falling)
        {
            SetSubState(PlayerStateFactory.FallState());
        }

        ProccessJumpInput(PlayerContextManager.JumpInput);
    }
    public override void InitializeSubStates()
    {
        if (PlayerContextManager.PerformingJump)
        {
            SetSubState(PlayerStateFactory.JumpState());            
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Enter))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Enter);
            }
        }
    }

    public override void OnCollisionStay(Collision2D collision)
    {

    }

    public override void OnCollisionExit2D(Collision2D collision)
    {

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Enter))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Enter);
            }
        }
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Stay))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Stay);
            }
        }

        if (!PlayerContextManager.Damaged)
        {
            SwitchState(PlayerStateFactory.GroundedState());
        }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Exit))
            {
                interactable.SetInteraction(PlayerContextManager.gameObject, EInteractionType.Exit);
            }
        }
    }

    protected override void ProccessJumpInput(bool actioninput)
    {
        if (actioninput && _coyoteTime > 0)
        {
            PlayerContextManager.PerformingJump = true;
            PlayerContextManager.Falling = false;

            PlayerContextManager.Rigidbody.velocity = new Vector2(PlayerContextManager.Rigidbody.velocity.x, 0);

            SetSubState(PlayerStateFactory.JumpState());
        }
    }
}
