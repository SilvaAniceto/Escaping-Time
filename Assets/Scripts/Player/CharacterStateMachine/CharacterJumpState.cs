using UnityEngine;

public class CharacterJumpState : CharacterAbstractState
{
    public CharacterJumpState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubStates();

        PlayerContextManager.GroundChecker.enabled = false;

        PlayerContextManager.Rigidbody.gravityScale = 1f;

        PlayerContextManager.Rigidbody.linearVelocity = new Vector2(PlayerContextManager.Rigidbody.linearVelocity.x, 0);

        PlayerContextManager.Rigidbody.AddForce(new Vector2(PlayerContextManager.Rigidbody.linearVelocity.x, 6.28f), ForceMode2D.Impulse);
    }
    public override void UpdateState()
    {
        PlayerContextManager.Rigidbody.gravityScale = Mathf.Lerp(PlayerContextManager.Rigidbody.gravityScale, 3.14f, Time.deltaTime);

        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void LateUpdateState()
    {
        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.JUMP_ANIMATION);
    }
    public override void ExitState()
    {
        
    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.VerticalVelocity < 0.00f)
        {
            SwitchState(PlayerStateFactory.FallState());
        }
    }
    public override void InitializeSubStates()
    {
        if (PlayerContextManager.MoveInput != 0)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
        else if (PlayerContextManager.MoveInput == 0)
        {
            SetSubState(PlayerStateFactory.IdleState());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
