using UnityEngine;

public class CharacterResetState : CharacterAbstractState
{
    public CharacterResetState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        CharacterAnimationManager.SetDisabledAnimation();

        CharacterContextManager.HorizontalSpeed = 0.00f;
        CharacterContextManager.VerticalSpeed = 0.00f;
        CharacterContextManager.HorizontalSpeedOvertime = 0.00f;

        GameStateTransitionManager.OnFadeInEnd.AddListener(() =>
        {
            CharacterAnimationManager.SetIdleAnimation();
            CharacterContextManager.EnableCharacterContext();
        });

        GameStateTransitionManager.OnFadeOutEnd.AddListener(() =>
        {
            CharacterContextManager.transform.position = CharacterContextManager.SpawningPosition;
            CharacterContextManager.OnResetState?.Invoke();
            GameStateTransitionManager.FadeIn();
        });

        GameStateTransitionManager.FadeOut();
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
    public override void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public override void OnCollisionStay(Collision2D collision)
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
