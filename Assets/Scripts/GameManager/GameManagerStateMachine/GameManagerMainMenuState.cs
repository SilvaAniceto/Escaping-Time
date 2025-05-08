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
            GameManagerContext.TargetScene = GameManagerContext.GameConfig.DefaultScene;
            SwitchState(GameManagerStateFactory.GameSaveMenuState());
        });

        GameManagerContext.QuitButton.onClick.RemoveAllListeners();
        GameManagerContext.QuitButton.onClick.AddListener(GameManagerContext.QuitGame);

        GameManagerContext.GameManagerEventSystem.SetSelectedGameObject(GameManagerContext.StartButton.gameObject);

        GameManagerContext.ExitState = null;
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
