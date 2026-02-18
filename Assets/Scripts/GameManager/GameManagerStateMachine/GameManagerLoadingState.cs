public class GameManagerLoadingState : GameManagerAbstractState
{
    public GameManagerLoadingState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory) : base(gameContextManager, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnEnterLoadingState();
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameContextManager.OnExitLoadingState();
    }

    public override void CheckSwitchStates()
    {

    }

    public override void CheckSwitchSubStates()
    {

    }
}
