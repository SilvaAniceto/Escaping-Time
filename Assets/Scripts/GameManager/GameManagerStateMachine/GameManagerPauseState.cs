public class GameManagerPauseState : GameManagerAbstractState
{
    public GameManagerPauseState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameManagerContext, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.OnRunOrPauseStateChanged?.Invoke(false);

        GameManagerContext.PauseMenu.SetActive(true);

        GameManagerContext.ContinueButton.onClick.RemoveAllListeners();
        GameManagerContext.ContinueButton.onClick.AddListener(() =>
        {
            SwitchState(GameManagerContext.ExitState);
        });

        GameManagerContext.GameManagerEventSystem.SetSelectedGameObject(GameManagerContext.ContinueButton.gameObject);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameManagerContext.PauseMenu.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (GameUIInputsManager.Cancel)
        {
            SwitchState(GameManagerContext.ExitState);
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
