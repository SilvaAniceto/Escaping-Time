public class GameManagerPauseState : GameManagerAbstractState
{
    public GameManagerPauseState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory) : base(gameContextManager, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnEnterPauseState();
    }

    public override void UpdateState()
    {
        if (GameContextManager.UIManager.Navigating && GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
        {
            if (GameContextManager.UIManager.PauseMenu.activeInHierarchy)
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.UIManager.ContinueButton.gameObject);
            }
            else
            {
                if (GameContextManager.UIManager.ConfirmMainMenuButton.gameObject.activeInHierarchy)
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.UIManager.ConfirmMainMenuButton.gameObject);
                }

                if (GameContextManager.UIManager.ConfirmHubButton.gameObject.activeInHierarchy)
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.UIManager.ConfirmHubButton.gameObject);
                }
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.OnExitPauseState();
    }

    public override void CheckSwitchStates()
    {

    }

    public override void CheckSwitchSubStates()
    {

    }
}
