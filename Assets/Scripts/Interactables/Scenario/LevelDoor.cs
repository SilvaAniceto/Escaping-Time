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

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);
        Interactions.Add(EInteractionType.Exit);

        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                GameStateTransitionManager.OnFadeInEnd += (() =>
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
                if (interactionType == EInteractionType.Enter && GameContextManager.Instance)
                {
                    GameStateTransitionManager.OnFadeInEnd += (() =>
                    {
                        characterContextManager.CurrentState.CharacterAnimationManager.SetIdleAnimation();
                    });
                }
                break;
            case PointType.Finish:
                if (interactionType == EInteractionType.Enter && GameContextManager.Instance)
                {
                    GameContextManager.Instance.SetTimer = false;
                    characterContextManager.DisableCharacterContext();
                    SetClosingAnimation();

                    GameStateTransitionManager.OnFadeInEnd += (() =>
                    {
                        characterContextManager.EnableCharacterContext();
                    });

                    GameStateTransitionManager.OnFadeInStart += (() =>
                    {
                        characterContextManager.transform.position = GameContextManager.Instance.CharacterHubStartPosition;
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
        GameAudioManager.Instance.PlaySFX("Door");
    }
    public void EndClosing()
    {
        GameAudioManager.Instance.StopSFX();
        GameAudioManager.Instance.PlaySFX("Door_Close");

        switch (_type)
        {
            case PointType.None:
                break;
            case PointType.Start:
                if (GameContextManager.Instance)
                {
                    GameContextManager.Instance.SetTimer = true;
                    GameContextManager.Instance.CharacterContextManager.EnableCharacterContext();
                    GameAudioManager.Instance.PlayFadedBGM("Level_Loop", 1.6f);
                }
                break;
            case PointType.Finish:
                if (GameContextManager.Instance)
                {
                    StartCoroutine(DelaySetFinalScore());
                    GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);
                }
                break;
            default:
                break;
        }

        IEnumerator DelaySetFinalScore()
        {
            yield return new WaitForSeconds(1.5f);

            GameContextManager.Instance.StartScoreState();
        }
    }
}
