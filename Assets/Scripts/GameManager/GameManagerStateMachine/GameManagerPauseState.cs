public class GameManagerPauseState : GameManagerAbstractState
{
    public GameManagerPauseState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnRunOrPauseStateChanged?.Invoke(false);

        GameContextManager.GameUIManager.PauseMenu.SetActive(true);

        GameContextManager.GameUIManager.ContinueButton.onClick.RemoveAllListeners();
        GameContextManager.GameUIManager.ContinueButton.onClick.AddListener(() =>
        {
            GameContextManager.GameAudioManager.PlaySFX("Menu_Start");

            System.Action action = () =>
            {
                SwitchState(GameContextManager.ExitState);
            };

            GameContextManager.WaitFrameEnd(action);
        });

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.GameUIManager.ContinueButton.gameObject);
    }

    public override void UpdateState()
    {
        if (GameUIManager.Navigating && GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
        {
            if (GameContextManager.GameUIManager.PauseMenu.activeInHierarchy)
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.GameUIManager.ContinueButton.gameObject);
            }
            else
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.GameUIManager.ConfirmMainMenuButton.gameObject);
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.GameUIManager.PauseMenu.SetActive(false);
        GameContextManager.OnRunOrPauseStateChanged?.Invoke(true);
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.CharacterContextManager.CurrentState.PlayerInputManager.Cancel)
        {
            SwitchState(GameContextManager.ExitState);
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
