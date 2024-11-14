using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class TimedPlatform : MonoBehaviour, IInteractable
{
    [SerializeField, Range(1, 5)] private int _tileCount = 1;

    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Stay);

        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [ContextMenu("SetTile")]
    public void SetTile()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        _spriteRenderer.size = new Vector2(_tileCount, 1);
    }

    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType)
    {        
        if (Activated)
        {
            return;
        }

        StartCoroutine("SetTimedPlatformBehaviour");
    }

    IEnumerator SetTimedPlatformBehaviour()
    {
        Activated = true;
        yield return new WaitForSeconds(0.25f * _tileCount);
        _boxCollider.enabled = false;
        _spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.8f);
        _boxCollider.enabled = true;
        _spriteRenderer.color = Color.white;
        Activated = false;
    }
}
