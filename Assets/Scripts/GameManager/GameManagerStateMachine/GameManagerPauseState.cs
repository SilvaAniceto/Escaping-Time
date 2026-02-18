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
        if (GameUIManager.Instance.Navigating && GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
        {
            if (GameUIManager.Instance.PauseMenu.activeInHierarchy)
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.ContinueButton.gameObject);
            }
            else
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.ConfirmMainMenuButton.gameObject);
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.OnExitPauseState();
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.PlayerInputManager == null) return;

        if (GameContextManager.PlayerInputManager.Cancel)
        {
            SwitchState(GameContextManager.ExitState);
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
