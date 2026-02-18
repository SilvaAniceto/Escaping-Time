public class GameManagerScoreState : GameManagerAbstractState
{
    public GameManagerScoreState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory) : base(gameContextManager, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnEnterScoreState();
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameContextManager.OnExitScoreState();
    }

    public override void CheckSwitchStates()
    {
        if (GameUIManager.Instance.ConfirmActionButton.gameObject.activeInHierarchy)
        {
            if (GameUIManager.Instance.Confirm)
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