public class GameManagerRunState : GameManagerAbstractState
{
    public GameManagerRunState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameManagerContext, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.OnRunOrPauseStateChanged?.Invoke(true);

        GameManagerContext.ExitState = null;

        GameManagerContext.CharacterUI.SetLevelUIObjects();

        GameManagerContext.CharacterUI.gameObject.SetActive(true);
    }

    public override void UpdateState()
    {
       
    }

    public override void ExitState()
    {
        GameManagerContext.CharacterUI.gameObject.SetActive(false);

        GameManagerContext.SetLevelScore = false;
    }

    public override void CheckSwitchStates()
    {
        if (GameUIInputsManager.Cancel)
        {
            GameManagerContext.ExitState = GameManagerStateFactory.GameRunState();

            GameManagerContext.ExitLevelButtonText.text = "Back to Hub";
            GameManagerContext.ConfirmPanelText.text = "Quit to Hub?";

            GameManagerContext.ConfirmMainMenuButton.onClick.RemoveAllListeners();
            GameManagerContext.ConfirmMainMenuButton.onClick.AddListener(() =>
            {
                GameManagerContext.TargetScene = "Level_Hub";
                GameManagerContext.ExitState = GameManagerStateFactory.GameHubState();
                GameManagerContext.ConfirmPanel.SetActive(false);
                SwitchState(GameManagerStateFactory.GameLoadingState());
            });

            SwitchState(GameManagerStateFactory.GamePauseState());
        }

        if (GameManagerContext.SetLevelScore)
        {
            GameManagerContext.ExitState = GameManagerStateFactory.GameHubState();

            SwitchState(GameManagerStateFactory.GameScoreState());
        }
    }

    public override void CheckSwitchSubStates()
    {
        
    }
}
