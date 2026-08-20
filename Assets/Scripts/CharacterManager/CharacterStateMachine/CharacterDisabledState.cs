using UnityEngine;

public class CharacterDisabledState : CharacterAbstractState
{
    public CharacterDisabledState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, /*PlayerInputManager inputManager,*/ CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, /*inputManager,*/ animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (!CharacterAnimationManager.CharacterAnimator.enabled)
        {
            CharacterAnimationManager.CharacterAnimator.enabled = true;
        }

        CharacterAnimationManager.SetDisabledAnimation();

        CharacterContextManager.Rigidbody.bodyType = RigidbodyType2D.Kinematic;

        CharacterContextManager.PhysicsManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.PhysicsManager.JumpSpeed = 0.00f;
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
}
