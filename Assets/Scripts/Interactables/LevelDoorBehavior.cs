using System.Collections;
using System.Linq;
using UnityEngine;

public class LevelDoorBehavior : MonoBehaviour, IInteractableBehavior, IConfirmable
{
    public enum PointType
    {
        None,
        Start,
        Finish
    }

    [SerializeField] private PointType _type;
    private Animator _animator;
    private AudioSource _audioSource;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (_type == PointType.Start)
        {
            GameStateTransitionManager.OnFadeInEnd += (() =>
            {
                SetClosingAnimation();
            });
        }
    }

    private void OnDestroy()
    {
        GameStateTransitionManager.OnFadeInEnd -= (() =>
        {
            SetClosingAnimation();
        });
    }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                if (_interactionType.Contains(interactionType) && GameContextManager.Instance != null)
                {
                    GameStateTransitionManager.OnFadeInEnd += (() =>
                    {
                        context.CurrentState.CharacterAnimationManager.SetIdleAnimation();
                    });
                }
                break;
            case PointType.Finish:
                if (_interactionType.Contains(interactionType) && GameContextManager.Instance != null)
                {
                    StartClosing(context);
                }
                break;
        }
    }

    public void ConfirmInteraction(CharacterContextManager context)
    {

    }

    private void StartClosing(CharacterContextManager context)
    {
        if (context == null)
        {
            return;
        }

        GameContextManager.Instance.SetTimer = false;
        context.DisableCharacterContext();
        SetClosingAnimation();

        GameStateTransitionManager.OnFadeInEnd += (() =>
        {
            context.EnableCharacterContext();
        });

        GameStateTransitionManager.OnFadeInStart += (() =>
        {
            context.transform.position = GameContextManager.Instance.CharacterHubStartPosition;
        });
    }

    public void SetOpeningAnimation()
    {
        if (_animator != null)
        {
            _animator.Play("Opening");
        }
    }

    public void SetClosingAnimation()
    {
        if (_animator != null)
        {
            _animator.Play("Closing");
        }
    }

    public void StartClosing()
    {
        if (_audioSource != null)
        {
            GameContextManager.Instance.AudioManager.StopSFX(_audioSource);
            GameContextManager.Instance.AudioManager.PlaySFX("Door", _audioSource);
        }
    }

    public void EndClosing()
    {
        if (_audioSource != null)
        {
            GameContextManager.Instance.AudioManager.StopSFX(_audioSource);
            GameContextManager.Instance.AudioManager.PlaySFX("Door_Close", _audioSource);
        }

        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                if (GameContextManager.Instance != null)
                {
                    GameContextManager.Instance.SetTimer = true;
                    GameContextManager.Instance.CharacterContextManager.EnableCharacterContext();
                    GameContextManager.Instance.AudioManager.PlayFadedBGM("Level_Loop", 1.6f);
                }
                break;
            case PointType.Finish:
                if (GameContextManager.Instance != null)
                {
                    GameContextManager.Instance.AudioManager.StopFadedBGM(0.0f, 1.5f);
                    GameContextManager.Instance.StartCoroutine(DelaySetFinalScore());
                }
                break;
        }
    }

    private IEnumerator DelaySetFinalScore()
    {
        yield return new WaitForSeconds(1.5f);
        GameContextManager.Instance.StartScoreState();
    }
}