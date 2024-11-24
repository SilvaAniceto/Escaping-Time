using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawningState : CharacterAbstractState
{
    private float _spawningWaitTime;

    public CharacterSpawningState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory) : base(currentContextManager, stateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        _spawningWaitTime = 0.6f;

        PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.DISABLED_ANIMATION);
        PlayerContextManager.Rigidbody.bodyType = RigidbodyType2D.Static;
        PlayerContextManager.CharacterCollider.enabled = false;
        PlayerContextManager.GroundChecker.enabled = false;
    }
    public override void UpdateState()
    {
        PlayerContextManager.transform.position = Vector3.MoveTowards(PlayerContextManager.transform.position, PlayerContextManager.SpawningPosition, 10f * Time.deltaTime);

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
        PlayerContextManager.SpawningCharacter = false;
        PlayerContextManager.CharacterCollider.enabled = true;
        PlayerContextManager.GroundChecker.enabled = true;
        PlayerContextManager.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
    public override void CheckSwitchStates()
    {
        if (PlayerContextManager.transform.position == PlayerContextManager.SpawningPosition)
        {
            PlayerContextManager.CharacterAnimator.Play(PlayerContextManager.SPAWNING_ANIMATION);

            _spawningWaitTime -= Time.deltaTime;
            _spawningWaitTime = Mathf.Clamp01(_spawningWaitTime);

            if (_spawningWaitTime <= 0)
            {
                SwitchState(PlayerStateFactory.GroundedState());
            }
        }
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

    }

    public override void OnTriggerStay2D(Collider2D collision)
    {

    }

    public override void OnTriggerExit2D(Collider2D collision)
    {

    }
}
