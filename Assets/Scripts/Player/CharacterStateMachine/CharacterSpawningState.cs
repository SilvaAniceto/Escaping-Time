using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawningState : CharacterAbstractState
{
    private float _spawningWaitTime;

    public CharacterSpawningState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager) : base(currentContextManager, stateFactory, inputManager, animationManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        _spawningWaitTime = 0.6f;

        CharacterAnimationManager.SetDisabledAnimation();
        CharacterContextManager.CharacterCollider.enabled = false;
    }
    public override void UpdateState()
    {
        CharacterContextManager.transform.position = Vector3.MoveTowards(CharacterContextManager.transform.position, CharacterContextManager.SpawningPosition, 10f * Time.deltaTime);
    }
    public override void FixedUpdateState()
    {

    }

    public override void LateUpdateState()
    {
        
    }
    public override void ExitState()
    {
        CharacterContextManager.SpawningCharacter = false;
        CharacterContextManager.CharacterCollider.enabled = true;
        
        CharacterContextManager.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
    public override void CheckSwitchStates()
    {
        if (CharacterContextManager.transform.position == CharacterContextManager.SpawningPosition)
        {
            CharacterAnimationManager.SetSpawningAnimation();

            _spawningWaitTime -= Time.deltaTime;
            _spawningWaitTime = Mathf.Clamp01(_spawningWaitTime);

            if (_spawningWaitTime <= 0)
            {
                SwitchState(CharacterStateFactory.GroundedState());
            }
        }
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
