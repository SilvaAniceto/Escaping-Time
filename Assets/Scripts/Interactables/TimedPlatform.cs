using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class TimedPlatform : MonoBehaviour, IInteractable
{
    [SerializeField, Range(1, 5)] private int _tileCount = 1;

    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;

    private float _activeTimer = 0.00f;
    private float _resetingTimer = 0.00f;
    private bool _reseting = false;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; }
    public bool MovableObject { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Stay);

        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        GameManagerContext.OnRunOrPauseStateChanged.AddListener(InteractablePauseState);
    }

    private void Update()
    {
        if (Activated)
        {
            _activeTimer += Time.deltaTime;
            _activeTimer = Mathf.Round(_activeTimer * 100.00f) / 100.00f;

            if (_activeTimer >= 0.12f * _tileCount)
            {
                _resetingTimer = 0.00f;
                _reseting = true;
                Activated = false;
				_boxCollider.enabled = false;
				_spriteRenderer.color = Color.gray;
			}
        }
        else
        {
            if (_reseting)
            {
			    _resetingTimer += Time.deltaTime;
			    _resetingTimer = Mathf.Round(_resetingTimer * 100.00f) / 100.00f;

                if (_resetingTimer >= 0.8f)
                {
                    _activeTimer = 0.00f;
                    _reseting = false;
				    _boxCollider.enabled = true;
				    _spriteRenderer.color = Color.white;
			    }
            }
		}
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

    public void SetInteraction(CharacterContextManager characterContextManager)
    {        
        if (Activated)
        {
            return;
        }

        Activated = true;
        //StartCoroutine("SetTimedPlatformBehaviour");
    }

    IEnumerator SetTimedPlatformBehaviour()
    {
        Activated = true;
        yield return new WaitForSeconds(0.12f * _tileCount);
        _boxCollider.enabled = false;
        _spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.8f);
        _boxCollider.enabled = true;
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
