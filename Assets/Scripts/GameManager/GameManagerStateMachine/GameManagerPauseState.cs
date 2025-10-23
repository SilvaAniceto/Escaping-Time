public class GameManagerPauseState : GameManagerAbstractState
{
    public GameManagerPauseState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnRunOrPauseStateChanged?.Invoke(false);

        GameContextManager.PauseMenu.SetActive(true);

        GameContextManager.ContinueButton.onClick.RemoveAllListeners();
        GameContextManager.ContinueButton.onClick.AddListener(() =>
        {
            GameAudioManager.Instance.PlaySFX("Menu_Start");

            System.Action action = () =>
            {
                SwitchState(GameContextManager.ExitState);
            };

            GameContextManager.WaitFrameEnd(action);
        });

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.ContinueButton.gameObject);
    }

    public override void UpdateState()
    {
        if (GameUIInputsManager.Navigating && GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
        {
            if (GameContextManager.PauseMenu.activeInHierarchy)
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.ContinueButton.gameObject);
            }
            else
            {
                GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.ConfirmMainMenuButton.gameObject);
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.PauseMenu.SetActive(false);
        GameContextManager.OnRunOrPauseStateChanged?.Invoke(true);
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.CharacterContextManager.CurrentState.PlayerInputManager.Cancel)
        {
            SwitchState(GameContextManager.ExitState);
        }
    }

    public override void CheckSwitchSubStates()
    {

    }
}
