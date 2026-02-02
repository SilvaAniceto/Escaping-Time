using System.Collections;
using UnityEngine;

public class LevelDoor : InteractableItem
{
    public enum PointType
    {
        None,
        Start,
        Finish
    }

    [SerializeField] private PointType _type;

    private GameContextManager _gameContextManager;

    public override void Awake()
    {
        base.Awake();

        _gameContextManager = FindAnyObjectByType<GameContextManager>();

        Interactions.Add(EInteractionType.Enter);
        Interactions.Add(EInteractionType.Exit);

        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                GameStateTransitionManager.OnFadeInEnd.AddListener(() =>
                {
                    SetClosingAnimation();
                });
                break;
            case PointType.Finish:
                break;
        }
    }
    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                if (interactionType == EInteractionType.Enter && _gameContextManager)
                {
                    GameStateTransitionManager.OnFadeInEnd.AddListener(() =>
                    {
                        characterContextManager.CurrentState.CharacterAnimationManager.SetIdleAnimation();
                    });
                }
                break;
            case PointType.Finish:
                if (interactionType == EInteractionType.Enter && _gameContextManager)
                {
                    _gameContextManager.SetTimer = false;
                    characterContextManager.DisableCharacterContext();
                    SetClosingAnimation();

                    GameStateTransitionManager.OnFadeInEnd.AddListener(() =>
                    {
                        characterContextManager.EnableCharacterContext();
                    });

                    GameStateTransitionManager.OnFadeInStart.AddListener(() =>
                    {
                        characterContextManager.transform.position = _gameContextManager.CharacterHubStartPosition;
                    });
                }
                break;
            default:
                break;
        }
    }
    public override void ConfirmInteraction()
    {

    }
    public override void InteractablePauseState(bool value)
    {
        base.InteractablePauseState(value);
    }

    public void SetOpeningAnimation()
    {
        Animator.Play("Opening");
    }
    public void SetClosingAnimation()
    {
        Animator.Play("Closing");
    }
    public void StartClosing()
    {
        _gameContextManager.GameAudioManager.PlaySFX("Door");
    }
    public void EndClosing()
    {
        _gameContextManager.GameAudioManager.StopSFX();
        _gameContextManager.GameAudioManager.PlaySFX("Door_Close");

        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                if (_gameContextManager)
                {
                    _gameContextManager.SetTimer = true;
                    _gameContextManager.CharacterContextManager.EnableCharacterContext();
                    _gameContextManager.GameAudioManager.PlayFadedBGM("Level_Loop", 1.6f);
                }
                break;
            case PointType.Finish:
                if (_gameContextManager)
                {
                    StartCoroutine(DelaySetFinalScore());
                    _gameContextManager.GameAudioManager.StopFadedBGM(0.0f, 1.5f);
                }
                break;
            default:
                break;
        }

        IEnumerator DelaySetFinalScore()
        {
            yield return new WaitForSeconds(1.5f);

            _gameContextManager.ScoreManager.SetFinalScore();
        }
    }
}
