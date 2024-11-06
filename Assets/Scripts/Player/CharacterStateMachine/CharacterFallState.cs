using UnityEngine;

public class CharacterFallState : CharacterAbstractState
{
    public CharacterFallState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        if (!PlayerContextManager.PerformingJump)
        {
            PlayerContextManager.Rigidbody.gravityScale = 3.14f;
        }

        PlayerContextManager.Falling = true;
        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.FALL_ANIMATION);

        InitializeSubStates();
    }
    public override void UpdateState()
    {
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
