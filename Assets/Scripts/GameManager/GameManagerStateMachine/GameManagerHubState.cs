public class GameManagerHubState : GameManagerAbstractState
{
    public GameManagerHubState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.GameUIManager.SetHubUIObjects();

        GameContextManager.GameUIManager.SetScoreDisplay(GameContextManager.ScoreManager.MasterScore);

        GameContextManager.GameUIManager.CharacterUIManager.SetActive(true);

        GameContextManager.OnHubState?.Invoke(GameContextManager);

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(null);

        GameContextManager.GameAudioManager.PlayFadedBGM("Hub_Loop", 1.6f);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameContextManager.LoadLevel = false;
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.LoadLevel)
        {
            GameContextManager.ExitState = GameManagerStateFactory.GameRunState();

            SwitchState(GameManagerStateFactory.GameLoadingState());

            GameContextManager.SaveSystem.SaveGame();
        }

        if (GameContextManager.PlayerInputManager.Cancel)
        {
            GameContextManager.ExitState = GameManagerStateFactory.GameHubState();

            GameContextManager.GameUIManager.ExitLevelButtonText.text = "Back to Main Menu";
            GameContextManager.GameUIManager.ConfirmPanelText.text = "Quit to Main Menu?";

            GameContextManager.GameUIManager.ExitLevelButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.ExitLevelButton.onClick.AddListener(() =>
            {
                GameContextManager.GameAudioManager.PlaySFX("Menu_Click");

                System.Action action = () =>
                {
                    GameContextManager.GameUIManager.ConfirmPanel.SetActive(true);
                    GameContextManager.GameUIManager.PauseMenu.SetActive(false);
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.GameUIManager.ConfirmMainMenuButton.gameObject);
                };

                GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Click"));
            });

            GameContextManager.GameUIManager.ConfirmMainMenuButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.ConfirmMainMenuButton.onClick.AddListener(() =>
            {
                GameContextManager.GameAudioManager.PlaySFX("Menu_Start");
                GameContextManager.SaveSystem.SaveGame();
                GameContextManager.TargetScene = "MainMenu";
                GameContextManager.ExitState = GameManagerStateFactory.GameMainMenuState();

                GameStateTransitionManager.OnFadeOutEnd.AddListener(() => 
                {
                    GameContextManager.OnQuitToMainMenu();

                    System.Action action = () =>
                    {
                        SwitchState(GameManagerStateFactory.GameLoadingState());
                    };
                    GameContextManager.GameUIManager.ConfirmPanel.SetActive(false);

                    GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Start"));
                });

                GameContextManager.GameAudioManager.StopFadedBGM(0.00f, 1.00f);
                GameStateTransitionManager.FadeOut();
            });

            SwitchState(GameManagerStateFactory.GamePauseState());
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
