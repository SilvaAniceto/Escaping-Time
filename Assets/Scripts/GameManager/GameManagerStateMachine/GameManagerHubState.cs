public class GameManagerHubState : GameManagerAbstractState
{
    public GameManagerHubState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory) : base(gameContextManager, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnEnterHubState();
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameContextManager.LoadLevel = false;
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.LoadLevel)
        {
            GameContextManager.ExitState = GameManagerStateFactory.GameRunState();

            SwitchState(GameManagerStateFactory.GameLoadingState());

            GameSaveSystem.Instance.SaveGame();
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
