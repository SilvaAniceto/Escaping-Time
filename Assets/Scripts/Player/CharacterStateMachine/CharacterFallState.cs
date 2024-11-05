using UnityEngine;

public class CharacterFallState : CharacterAbstractState
{
    public CharacterFallState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = false;
    }

    public override void EnterState()
    {
        InitializeSubStates();
        PlayerContextManager.Rigidbody.gravityScale = Mathf.PI;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        PlayerContextManager.Rigidbody.MovePosition(new Vector3(PlayerContextManager.Rigidbody.position.x, PlayerContextManager.Rigidbody.position.y, 0) + PlayerContextManager.MoveInput);
        Debug.Log("FALLING");
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
