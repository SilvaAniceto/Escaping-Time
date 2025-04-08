using UnityEngine;

public class GameManagerMainMenuState : GameManagerAbstractState
{
    public GameManagerMainMenuState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory) : base(gameManagerContext, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.MainMenu.SetActive(true);

        GameManagerContext.StartButton.onClick.RemoveAllListeners();
        GameManagerContext.StartButton.onClick.AddListener(() =>
        {
            GameManagerContext.TargetScene = GameManagerContext.GameConfig.SceneList[0];
            GameManagerContext.ExitState = GameManagerStateFactory.GameRunState();
            SwitchState(GameManagerStateFactory.GameLoadingState());
        });

        GameManagerContext.GameManagerEventSystem.SetSelectedGameObject(GameManagerContext.StartButton.gameObject);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameManagerContext.MainMenu.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        
    }

    public override void CheckSwitchSubStates()
    {

    }
}
