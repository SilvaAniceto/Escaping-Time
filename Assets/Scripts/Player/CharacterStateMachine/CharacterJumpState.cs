using UnityEngine;

public class CharacterJumpState : CharacterAbstractState
{
    public CharacterJumpState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        InitializeSubStates();
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        PlayerContextManager.Rigidbody.MovePosition((new Vector3(PlayerContextManager.Rigidbody.position.x, 2, 0)) * Time.deltaTime);
    }
    public override void LateUpdateState()
    {

    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.MoveInput != Vector3.zero)
        {
            SetSubState(PlayerStateFactory.MoveState());
        }
        else
        {
            SetSubState(PlayerStateFactory.IdleState());
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
