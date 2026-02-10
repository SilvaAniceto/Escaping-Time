public class GameManagerScoreState : GameManagerAbstractState
{
    public GameManagerScoreState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.GameUIManager.ScorePanel.SetActive(true);

        GameContextManager.ScoreManager.SetScoreManager();

        GameContextManager.TargetScene = "Level_Hub";

        GameContextManager.GameUIManager.ConfirmActionButton.onClick.RemoveAllListeners();
        GameContextManager.GameUIManager.ConfirmActionButton.onClick.AddListener(() =>
        {
            GameContextManager.GameAudioManager.PlaySFX("Menu_Click");

            System.Action action = () =>
            {
                GameUIManager.SetConfirmAction();
            };

            GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Click"));
        });
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameContextManager.GameUIManager.ScorePanel.SetActive(false);
        GameContextManager.LoadLevel = false;
        GameContextManager.GameUIManager.ConfirmActionButton.onClick.RemoveAllListeners();
        GameContextManager.GameUIManager.ConfirmActionButton.gameObject.SetActive(false);
        GameUIManager.SetConfirmAction();
        GameContextManager.SaveSystem.SaveGame();
        GameContextManager.ScoreManager.ResetPlayerScorePoints();
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.GameUIManager.ConfirmActionButton.gameObject.activeInHierarchy)
        {
            if (GameUIManager.Confirm)
            {
                GameContextManager.ExitState = GameManagerStateFactory.GameHubState();

                SwitchState(GameManagerStateFactory.GameLoadingState());
            }
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}