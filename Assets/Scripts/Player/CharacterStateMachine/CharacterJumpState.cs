using UnityEngine;

public class CharacterJumpState : CharacterAbstractState
{
    private const float _jumpSpeed = 6.28f;

    public CharacterJumpState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.JUMP_ANIMATION);

        PlayerContextManager.Rigidbody.gravityScale = 1f;
        PlayerContextManager.Rigidbody.AddForce(new Vector2(PlayerContextManager.Rigidbody.velocity.x, _jumpSpeed), ForceMode2D.Impulse);

        InitializeSubStates();
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

    }
    public override void ExitState()
    {
        PlayerContextManager.PerformingJump = false;
    }
    public override void CheckSwitchStates()
    {
        ProccessMoveInput(PlayerContextManager.MoveInput);
    }
    public override void InitializeSubStates()
    {
        
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public override void OnCollisionStay(Collision collision)
    {

    }

    public override void OnCollisionExit2D(Collision2D collision)
    {

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {

    }

    public override void OnTriggerExit2D(Collider2D collision)
    {

    }
    protected override void ProccessMoveInput(float moveInput)
    {
        base.ProccessMoveInput(moveInput);

        if (moveInput != 0)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
    }
}
