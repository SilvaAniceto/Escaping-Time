using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManagerRunState : GameManagerAbstractState
{
    public GameManagerRunState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory) : base(gameContextManager, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnEnterRunState();
    }

    public override void UpdateState()
    {
       
    }

    public override void ExitState()
    {
        GameContextManager.OnExitRunState();
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void CheckSwitchSubStates()
    {
        
    }
}
