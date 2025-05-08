using UnityEngine;

public class GameManagerSaveMenuState : GameManagerAbstractState
{
    public GameManagerSaveMenuState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory) : base(gameManagerContext, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.SaveMenu.SetActive(true);

        GameManagerContext.BackButton.gameObject.SetActive(true);

        GameManagerContext.GameSaveSystem.OnLaunchGame.RemoveAllListeners();
        GameManagerContext.GameSaveSystem.OnLaunchGame.AddListener(() =>
        {
            SwitchState(GameManagerStateFactory.GameLoadingState());
        });

        GameManagerContext.ExitState = GameManagerStateFactory.GameRunState();
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameManagerContext.SaveMenu.SetActive(false);

        GameManagerContext.BackButton.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (GameManagerContext.GameSaveSystem.SlotIsSelected)
        {
            GameManagerContext.BackButton.onClick.RemoveAllListeners();
            GameManagerContext.BackButton.onClick.AddListener(GameManagerContext.GameSaveSystem.HideOptions);
        }
        else
        {
            GameManagerContext.BackButton.onClick.RemoveAllListeners();
            GameManagerContext.BackButton.onClick.AddListener(() =>
            {
                SwitchState(GameManagerStateFactory.GameMainMenuState());
            });
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
