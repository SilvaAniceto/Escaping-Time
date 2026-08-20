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
                if (_interactionType.Contains(interactionType))
                {
                    GameStateTransitionManager.OnFadeInEnd += (() =>
                    {
                        context.CurrentState.CharacterAnimationManager.SetIdleAnimation();
                    });
                }
                break;
            case PointType.Finish:
                if (_interactionType.Contains(interactionType))
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

        ServiceLocator.GameFlowManager.SetTimer = false;
        context.DisableCharacterContext();
        SetClosingAnimation();

        GameStateTransitionManager.OnFadeInEnd += (() =>
        {
            context.EnableCharacterContext();
        });

        GameStateTransitionManager.OnFadeInStart += (() =>
        {
            context.transform.position = ServiceLocator.GameFlowManager.CharacterHubStartPosition;
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
            ServiceLocator.AudioManager.StopSFX(_audioSource);
            ServiceLocator.AudioManager.PlaySFX("Door", _audioSource);
        }
    }

    public void EndClosing()
    {
        if (_audioSource != null)
        {
            ServiceLocator.AudioManager.StopSFX(_audioSource);
            ServiceLocator.AudioManager.PlaySFX("Door_Close", _audioSource);
        }

        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                ServiceLocator.GameFlowManager.SetTimer = true;
                ServiceLocator.GameFlowManager.CharacterContextManager.EnableCharacterContext();
                ServiceLocator.AudioManager.PlayFadedBGM("Level_Loop", 1.6f);
                break;
            case PointType.Finish:
                ServiceLocator.AudioManager.StopFadedBGM(0.0f, 1.5f);
                StartCoroutine(DelaySetFinalScore());
                break;
        }
    }

    private IEnumerator DelaySetFinalScore()
    {
        yield return new WaitForSeconds(1.5f);
        ServiceLocator.GameFlowManager.StartScoreState();
    }
}