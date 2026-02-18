public class GameManagerMainMenuState : GameManagerAbstractState
{
    public GameManagerMainMenuState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory) : base(gameContextManager, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnEnterMainMenuState();
    }

    public override void UpdateState()
    {
        if (GameUIManager.Instance.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.StartButton.gameObject);
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.OnExitMainMenuState();
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void CheckSwitchSubStates()
    {

    }
}
