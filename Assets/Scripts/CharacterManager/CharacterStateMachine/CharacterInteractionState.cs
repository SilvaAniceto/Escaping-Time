using UnityEngine;

public class CharacterInteractionState : CharacterAbstractState
{
    public CharacterInteractionState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        
    }
    public override void UpdateState()
    {
        
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
    public override void CheckSwitchSubStates()
    {
        
    }

    public override Quaternion CurrentLookRotation()
    {
        return new Quaternion();
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        SwitchState(CharacterStateFactory.GroundedState());
    }
}
