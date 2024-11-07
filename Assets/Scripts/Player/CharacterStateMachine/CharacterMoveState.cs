using UnityEngine;

public class CharacterMoveState : CharacterAbstractState
{
    private const float _moveSpeed = 4.71f;

    public CharacterMoveState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        if (!PlayerContextManager.PerformingJump && !PlayerContextManager.Falling)
        {
            PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.RUN_ANIMATION);
        }
    }
    public override void UpdateState()
    {
        float moveInput = PlayerContextManager.MoveInput;

        PlayerContextManager.Rigidbody.velocity = new Vector2(moveInput * _moveSpeed, PlayerContextManager.Rigidbody.velocity.y);

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
