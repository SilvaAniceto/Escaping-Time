public class GameManagerMainMenuState : GameManagerAbstractState
{
    public GameManagerMainMenuState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.GameContextAudioListener.enabled = true;

        GameContextManager.OnRunOrPauseStateChanged.RemoveAllListeners();

        GameContextManager.GameUIManager.MainMenu.SetActive(true);

        GameContextManager.GameUIManager.StartButton.gameObject.SetActive(true);
        GameContextManager.GameUIManager.QuitButton.gameObject.SetActive(true);
    
        GameContextManager.GameUIManager.StartButton.onClick.RemoveAllListeners();
        GameContextManager.GameUIManager.StartButton.onClick.AddListener(() =>
        {
            GameContextManager.TargetScene = "Level_Hub";

            GameContextManager.GameAudioManager.StopSFX();
            GameContextManager.GameAudioManager.PlaySFX("Menu_Click");

            GameContextManager.GameUIManager.StartButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.QuitButton.gameObject.SetActive(false);

            System.Action action = () =>
            {
                SwitchState(GameManagerStateFactory.GameSaveMenuState());
            };

            GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Click"));

        });

        GameContextManager.GameUIManager.QuitButton.onClick.RemoveAllListeners();
        GameContextManager.GameUIManager.QuitButton.onClick.AddListener(() =>
        {
            GameContextManager.GameAudioManager.PlaySFX("Menu_Click");

            GameContextManager.GameUIManager.QuitButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.StartButton.gameObject.SetActive(false);

            GameContextManager.WaitFrameEnd(() =>
            {
                GameContextManager.QuitGame();
            });
        });

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.GameUIManager.StartButton.gameObject);

        GameContextManager.ExitState = null;

        GameContextManager.GameAudioManager.PlayFadedBGM("Main_Menu", 2.0f);
    }

    public override void UpdateState()
    {
        if (GameUIManager.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.GameUIManager.StartButton.gameObject);
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.GameUIManager.MainMenu.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void CheckSwitchSubStates()
    {

    }
}
