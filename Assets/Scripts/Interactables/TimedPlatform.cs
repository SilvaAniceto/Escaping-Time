using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TimedPlatform : MonoBehaviour, IInteractable
{
    [SerializeField, Range(1, 5)] private int _tileCount = 1;

    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Stay);

        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _animator.enabled = false;

        GameManagerContext.OnRunOrPauseStateChanged.AddListener(InteractablePauseState);
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

    public void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {        
        if (Activated || characterContextManager.transform.position.y < transform.position.y)
        {
            return;
        }

        SetTimedPlatformBehaviour();
    }

    private async void SetTimedPlatformBehaviour()
    {
        Activated = true;

        _animator.enabled = true;

        await Task.Delay(_tileCount * 167);

        _animator.enabled = false;
        _collider.enabled = false;
        _spriteRenderer.color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.50f);

        await Task.Delay(800);

        _collider.enabled = true;
        _spriteRenderer.color = Color.white;

        Activated = false;
    }
    public void ConfirmInteraction()
    {

    }

    public void InteractablePauseState(bool value)
    {
        this.enabled = value;
    }
}
