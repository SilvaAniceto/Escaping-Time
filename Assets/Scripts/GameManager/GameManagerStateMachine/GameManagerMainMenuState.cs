public class GameManagerMainMenuState : GameManagerAbstractState
{
    public GameManagerMainMenuState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameManagerContext, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.MainMenu.SetActive(true);

        GameManagerContext.StartButton.onClick.RemoveAllListeners();
        GameManagerContext.StartButton.onClick.AddListener(() =>
        {
            GameManagerContext.TargetScene = "Level_Hub";
            SwitchState(GameManagerStateFactory.GameSaveMenuState());
        });

        GameManagerContext.QuitButton.onClick.RemoveAllListeners();
        GameManagerContext.QuitButton.onClick.AddListener(GameManagerContext.QuitGame);

        GameManagerContext.GameManagerEventSystem.SetSelectedGameObject(GameManagerContext.StartButton.gameObject);

        GameManagerContext.ExitState = null;

        GameManagerContext.CharacterStartPosition = UnityEngine.Vector2.zero;
    }

    public override void UpdateState()
    {
        if (GameManagerContext.GameManagerEventSystem.currentSelectedGameObject == null)
        {
            if (GameUIInputsManager.Navigating)
            {
                GameManagerContext.GameManagerEventSystem.SetSelectedGameObject(GameManagerContext.StartButton.gameObject);
            }
        }
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
