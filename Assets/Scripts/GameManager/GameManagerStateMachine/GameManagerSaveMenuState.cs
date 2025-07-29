using System.Linq;
using static GameSaveSystem;

public class GameManagerSaveMenuState : GameManagerAbstractState
{
    public GameManagerSaveMenuState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameManagerContext, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.SaveMenu.SetActive(true);

        GameManagerContext.BackButton.gameObject.SetActive(true);

        GameManagerContext.SaveSystem.OnLaunchGame.RemoveAllListeners();
        GameManagerContext.SaveSystem.OnLaunchGame.AddListener(() =>
        {
            SwitchState(GameManagerStateFactory.GameLoadingState());
        });

        GameManagerContext.ExitState = GameManagerStateFactory.GameHubState();
    }

    public override void UpdateState()
    {
        if (GameManagerContext.CurrentState.GameUIInputsManager.Start)
        {
            GameManagerContext.GameManagerEventSystem.SetSelectedGameObject(GameManagerContext.SaveSystem.SelectSaveButton.gameObject);
        }
    }

    public override void ExitState()
    {
        GameManagerContext.SaveMenu.SetActive(false);

        GameManagerContext.BackButton.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (GameManagerContext.SaveSystem.SlotIsSelected)
        {
            GameManagerContext.BackButton.onClick.RemoveAllListeners();
            GameManagerContext.BackButton.onClick.AddListener(GameManagerContext.SaveSystem.HideOptions);
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
