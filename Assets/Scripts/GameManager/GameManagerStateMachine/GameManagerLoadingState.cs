using UnityEngine;

public class GameManagerLoadingState : GameManagerAbstractState
{
    public GameManagerLoadingState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory) : base(gameManagerContext, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.LoadingScreen.SetActive(true);

        GameManagerContext.LoadingScenes = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GameManagerContext.TargetScene);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameManagerContext.LoadingScreen.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (GameManagerContext.LoadingScenes != null && GameManagerContext.LoadingScenes.progress >= 1)
        {
            SwitchState(GameManagerContext.ExitState);
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
