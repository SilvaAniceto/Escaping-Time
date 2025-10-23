public class GameManagerMainMenuState : GameManagerAbstractState
{
    public GameManagerMainMenuState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.GameContextAudioListener.enabled = true;

        GameContextManager.OnRunOrPauseStateChanged.RemoveAllListeners();

        GameContextManager.MainMenu.SetActive(true);

        GameContextManager.StartButton.gameObject.SetActive(true);
        GameContextManager.QuitButton.gameObject.SetActive(true);

        GameContextManager.StartButton.onClick.RemoveAllListeners();
        GameContextManager.StartButton.onClick.AddListener(() =>
        {
            GameContextManager.TargetScene = "Level_Hub";

            GameAudioManager.Instance.StopSFX();
            GameAudioManager.Instance.PlaySFX("Menu_Click");

            GameContextManager.StartButton.onClick.RemoveAllListeners();
            GameContextManager.QuitButton.gameObject.SetActive(false);

            System.Action action = () =>
            {
                SwitchState(GameManagerStateFactory.GameSaveMenuState());
            };

            GameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Click"));

        });

        GameContextManager.QuitButton.onClick.RemoveAllListeners();
        GameContextManager.QuitButton.onClick.AddListener(() =>
        {
            GameAudioManager.Instance.PlaySFX("Menu_Click");

            GameContextManager.QuitButton.onClick.RemoveAllListeners();
            GameContextManager.StartButton.gameObject.SetActive(false);

            GameContextManager.WaitFrameEnd(() =>
            {
                GameContextManager.QuitGame();
            });
        });

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.StartButton.gameObject);

        GameContextManager.ExitState = null;

        GameAudioManager.Instance.PlayFadedBGM("Main_Menu", 2.0f);
    }

    public override void UpdateState()
    {
        if (GameUIInputsManager.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.StartButton.gameObject);
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.MainMenu.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void CheckSwitchSubStates()
    {

    }
}
