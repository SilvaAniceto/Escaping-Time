public interface IGameStateManager
{
    GameManagerAbstractState CurrentState { get; }
    GameManagerAbstractState ExitState { get; set; }
    void PauseGameOnHubState();
    void PauseOnRunState();
}