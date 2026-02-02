using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TimedPlatform : InteractableItem
{
    [SerializeField, Range(1, 5)] private int _tileCount = 1;

    private SpriteRenderer _spriteRenderer;

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Stay);

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnDestroy()
    {

    }

    [ContextMenu("SetTile")]
    public void SetTile()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        _spriteRenderer.size = new Vector2(_tileCount, _spriteRenderer.size.y);
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {        
        if (Activated || characterContextManager.transform.position.y < transform.position.y)
        {
            return;
        }

        StartCoroutine(SetTimedPlatformBehaviour(characterContextManager.GameContextManager));
    }

    IEnumerator SetTimedPlatformBehaviour(GameContextManager gameContextManager)
    {
        Activated = true;

        gameContextManager.GameAudioManager.StopSFX(AudioSource);
        gameContextManager.GameAudioManager.PlaySFX("Enter_Platform", AudioSource);
        Animator.Play("TimedPlatform");

        yield return new WaitForSeconds((2.0f / _tileCount));

        Animator.Play("Unactive");
        Collider.enabled = false;

        gameContextManager.GameAudioManager.StopSFX(AudioSource);
        gameContextManager.GameAudioManager.PlaySFX("End_Platform", AudioSource);

        yield return new WaitForSeconds(2);

        Animator.Play("Base");
        Collider.enabled = true;

        Activated = false;
    }
    public override void ConfirmInteraction()
    {

    }

    public override void InteractablePauseState(bool value)
    {
        base.InteractablePauseState(value);
    }
}
