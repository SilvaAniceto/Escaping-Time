using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterDamagedState : CharacterAbstractState
{

    public CharacterDamagedState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        PlayerContextManager.GroundChecker.enabled = true;

        PlayerContextManager.Damaged = false;

        PlayerContextManager.Rigidbody.linearVelocity = Vector3.zero;

        PlayerContextManager.Rigidbody.gravityScale = 1f;

        PlayerContextManager.Rigidbody.AddForce(new Vector2(PlayerContextManager.Rigidbody.linearVelocity.x, 4.71f), ForceMode2D.Impulse);
    }
    public override void UpdateState()
    {
        PlayerContextManager.Rigidbody.gravityScale = Mathf.Lerp(PlayerContextManager.Rigidbody.gravityScale, 3.14f, Time.deltaTime);

        PlayerContextManager.Rigidbody.linearVelocity = new Vector2(PlayerContextManager.HitDirection.x * 3.14f, PlayerContextManager.VerticalVelocity);

        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {

    }

    public override void LateUpdateState()
    {
        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.HIT_ANIMATION);
    }
    public override void ExitState()
    {
        PlayerContextManager.Rigidbody.linearVelocity = Vector3.zero;
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

    public override void OnCollisionStay(Collision2D collision)
    {
        
    }

    public override void OnCollisionExit2D(Collision2D collision)
    {

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        SwitchState(PlayerStateFactory.GroundedState());
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {

    }
}
