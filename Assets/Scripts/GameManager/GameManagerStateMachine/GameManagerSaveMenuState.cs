public class GameManagerSaveMenuState : GameManagerAbstractState
{
    public GameManagerSaveMenuState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.SaveSystem.ShowSlots();

        GameContextManager.SaveMenu.SetActive(true);

        GameContextManager.BackButton.gameObject.SetActive(true);

        GameContextManager.SaveSystem.OnLaunchGame.RemoveAllListeners();
        GameContextManager.SaveSystem.OnLaunchGame.AddListener(() =>
        {
            GameStateTransitionManager.OnFadeOutEnd.AddListener(() =>
            {
                System.Action action = () =>
                {
                    SwitchState(GameManagerStateFactory.GameLoadingState());
                };

                GameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Start"));
            });

            GameStateTransitionManager.FadeOut();
        });

        GameContextManager.ExitState = GameManagerStateFactory.GameHubState();

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.SaveSystem.SaveSlots[0].slotButton.gameObject);
    }

    public override void UpdateState()
    {
        if (GameContextManager.CurrentState.GameUIInputsManager.Start)
        {
            GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.SaveSystem.SelectSaveButton.gameObject);
        }

        if (GameUIInputsManager.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
            {
                if (GameContextManager.SaveSystem.SlotIsSelected)
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.SaveSystem.SelectSaveButton.gameObject);
                }
                else
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.SaveSystem.SaveSlots[0].slotButton.gameObject);
                }
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.SaveMenu.SetActive(false);

        GameContextManager.BackButton.gameObject.SetActive(false);

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(null);
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.SaveSystem.SlotIsSelected)
        {
            GameContextManager.BackButton.onClick.RemoveAllListeners();
            GameContextManager.BackButton.onClick.AddListener(() =>
            {
                GameUIInputsManager.enabled = false;

                GameAudioManager.Instance.PlaySFX("Menu_Back");

                System.Action action = () =>
                {
                    GameContextManager.SaveSystem.HideOptions();
                };

                GameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Back"));
            });
        }
        else
        {
            GameContextManager.BackButton.onClick.RemoveAllListeners();
            GameContextManager.BackButton.onClick.AddListener(() =>
            {
                GameAudioManager.Instance.PlaySFX("Menu_Back");

                System.Action action = () =>
                {
                    SwitchState(GameManagerStateFactory.GameMainMenuState());
                };

                GameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Back"));
            });
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
