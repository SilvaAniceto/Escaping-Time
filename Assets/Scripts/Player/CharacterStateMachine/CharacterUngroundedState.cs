using UnityEngine;

public class CharacterUngroundedState : CharacterAbstractState
{
    public CharacterUngroundedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
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
        
    }
    public override void LateUpdateState()
    {

    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.JumpInput)
        {
            SetSubState(PlayerStateFactory.JumpState());
        }
        else
        {
            SetSubState(PlayerStateFactory.FallState());
        }
    }
    public override void InitializeSubStates()
    {
        
    }
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        SwitchState(PlayerStateFactory.GroundedState());
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
