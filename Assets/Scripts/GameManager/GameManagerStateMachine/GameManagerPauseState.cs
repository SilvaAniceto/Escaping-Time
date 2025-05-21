using UnityEngine;

public class GameManagerPauseState : GameManagerAbstractState
{
    public GameManagerPauseState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory) : base(gameManagerContext, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.OnRunOrPauseStateChanged?.Invoke(false);

        GameManagerContext.PauseMenu.SetActive(true);

        GameManagerContext.ContinueButton.onClick.RemoveAllListeners();
        GameManagerContext.ContinueButton.onClick.AddListener(() =>
        {
            SwitchState(GameManagerStateFactory.GameRunState());
        });

        GameManagerContext.ConfirmMainMenuButton.onClick.RemoveAllListeners();
        GameManagerContext.ConfirmMainMenuButton.onClick.AddListener(() =>
        {
            GameManagerContext.TargetScene = GameManagerContext.PlayeableCharacterSet.MainMenuScene;
            GameManagerContext.ExitState = GameManagerStateFactory.GameMainMenuState();
            GameManagerContext.ConfirmPanel.SetActive(false);
            SwitchState(GameManagerStateFactory.GameLoadingState());
            Object.Destroy(GameManagerContext.CharacterContextManager.gameObject);
            Object.Destroy(GameManagerContext.CameraBehaviourController.gameObject);
        });

        GameManagerContext.GameManagerEventSystem.SetSelectedGameObject(GameManagerContext.ContinueButton.gameObject);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        GameManagerContext.PauseMenu.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (GameManagerContext.PauseInput)
        {
            SwitchState(GameManagerStateFactory.GameRunState());
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
