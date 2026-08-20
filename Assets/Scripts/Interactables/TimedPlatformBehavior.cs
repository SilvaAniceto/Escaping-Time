using System.Collections;
using System.Linq;
using UnityEngine;

public class TimedPlatformBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField, Range(1, 5)] private int _tileCount = 1;

    private bool _activated = false;
    private Collider2D _collider;
    private Animator _animator;
    private AudioSource _audioSource;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Stay };
    public EInteractionType[] InteractionType { get => _interactionType; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        if (_activated || context.transform.position.y < transform.position.y)
        {
            return;
        }

        StartCoroutine(SetTimedPlatformBehaviour());
    }

    private IEnumerator SetTimedPlatformBehaviour()
    {
        _activated = true;

        ServiceLocator.AudioManager.StopSFX(_audioSource);
        ServiceLocator.AudioManager.PlaySFX("Enter_Platform", _audioSource);

        if (_animator != null)
        {
            _animator.Play("TimedPlatform");
        }

        yield return new WaitForSeconds(2.0f / _tileCount);

        if (_animator != null)
        {
            _animator.Play("Unactive");
        }

        _collider.enabled = false;

        ServiceLocator.AudioManager.StopSFX(_audioSource);
        ServiceLocator.AudioManager.PlaySFX("End_Platform", _audioSource);

        yield return new WaitForSeconds(2.0f);

        if (_animator != null)
        {
            _animator.Play("Base");
        }

        _collider.enabled = true;
        _activated = false;
    }

    [ContextMenu("SetTile")]
    public void SetTile()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.size = new Vector2(_tileCount, spriteRenderer.size.y);
        }
    }
}