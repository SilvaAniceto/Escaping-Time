using UnityEngine;

public class CharacterFallState : CharacterAbstractState
{
    public CharacterFallState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        CharacterContextManager.HorizontalTopSpeed = 6.30f;

        CharacterContextManager.CeilingChecker.enabled = false;
        CharacterContextManager.WallChecker.enabled = false;

        CharacterContextManager.FallSpeedOvertime = 0.00f;

    }
    public override void UpdateState()
    {
        CharacterContextManager.JumpSpeed = Mathf.Lerp(CharacterContextManager.FallStartSpeed, -24.00f, CharacterContextManager.GetFallSpeedLerpOvertime());
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void LateUpdateState()
    {
        if (!CharacterContextManager.DamageOnCoolDown)
        {
            CharacterAnimationManager.SetFallAnimation();
        }
    }
    public override void ExitState()
    {
        CharacterContextManager.JumpSpeed = 0.00f;
    }
    public override void CheckSwitchStates()
    {
        if (Grounded)
        {
            SwitchState(CharacterStateFactory.GroundedState());
        }
    }
    public override void CheckSwitchSubStates()
    {
        
    }
    public override Quaternion CurrentLookRotation()
    {
        return new Quaternion();
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay(Collision2D collision) { }

    public override void OnCollisionExit2D(Collision2D collision) { }

    public override void OnTriggerEnter2D(Collider2D collision) { }

    public override void OnTriggerStay2D(Collider2D collision) { }

    public override void OnTriggerExit2D(Collider2D collision) { }
}
