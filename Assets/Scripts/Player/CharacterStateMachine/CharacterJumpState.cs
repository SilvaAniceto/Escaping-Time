using UnityEngine;

public class CharacterJumpState : CharacterAbstractState
{
    public CharacterJumpState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        if (!PlayerContextManager.PerformingJump)
        {
            return;
        }

        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.JUMP_ANIMATION);

        PlayerContextManager.Rigidbody.gravityScale = 1f;
        PlayerContextManager.Rigidbody.AddForce(new Vector2(PlayerContextManager.Rigidbody.velocity.x, PlayerContextManager.JumpSpeed), ForceMode2D.Impulse);

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
        if (PlayerContextManager.MoveInput != 0)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
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
}
