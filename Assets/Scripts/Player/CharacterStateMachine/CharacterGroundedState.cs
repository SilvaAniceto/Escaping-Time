using UnityEngine;

public class CharacterGroundedState : CharacterAbstractState
{
    public CharacterGroundedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubStates();
        PlayerContextManager.Rigidbody.gravityScale = 1;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        Debug.Log("GROUNDED");
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

        if (PlayerContextManager.JumpInput)
        {
            SwitchState(PlayerStateFactory.UngroundedState());
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
