public class GameManagerSaveMenuState : GameManagerAbstractState
{
    public GameManagerSaveMenuState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.SaveSystem.ShowSlots();

        GameContextManager.GameUIManager.SaveMenu.SetActive(true);

        GameContextManager.GameUIManager.BackButton.gameObject.SetActive(true);

        GameContextManager.SaveSystem.OnLaunchGame.RemoveAllListeners();
        GameContextManager.SaveSystem.OnLaunchGame.AddListener(() =>
        {
            GameStateTransitionManager.OnFadeOutEnd.AddListener(() =>
            {
                System.Action action = () =>
                {
                    SwitchState(GameManagerStateFactory.GameLoadingState());
                };

                GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Start"));
            });

            GameStateTransitionManager.FadeOut();
        });

        GameContextManager.ExitState = GameManagerStateFactory.GameHubState();

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.SaveSlots[0].slotButton.gameObject);
    }

    public override void UpdateState()
    {
        if (GameContextManager.CurrentState.GameUIManager.Start)
        {
            GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.SelectSaveButton.gameObject);
        }

        if (GameUIManager.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
            {
                if (GameContextManager.SaveSystem.SlotIsSelected)
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.SelectSaveButton.gameObject);
                }
                else
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.SaveSlots[0].slotButton.gameObject);
                }
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.GameUIManager.SaveMenu.SetActive(false);

        GameContextManager.GameUIManager.BackButton.gameObject.SetActive(false);

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(null);
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.SaveSystem.SlotIsSelected)
        {
            GameContextManager.GameUIManager.BackButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.BackButton.onClick.AddListener(() =>
            {
                GameUIManager.enabled = false;

                GameContextManager.GameAudioManager.PlaySFX("Menu_Back");

                System.Action action = () =>
                {
                    GameContextManager.SaveSystem.HideOptions();
                };

                GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Back"));
            });
        }
        else
        {
            GameContextManager.GameUIManager.BackButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.BackButton.onClick.AddListener(() =>
            {
                GameContextManager.GameAudioManager.PlaySFX("Menu_Back");

                System.Action action = () =>
                {
                    SwitchState(GameManagerStateFactory.GameMainMenuState());
                };

                GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Back"));
            });
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
