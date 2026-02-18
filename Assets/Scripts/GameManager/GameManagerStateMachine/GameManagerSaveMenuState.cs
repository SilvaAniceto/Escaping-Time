public class GameManagerSaveMenuState : GameManagerAbstractState
{
    public GameManagerSaveMenuState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory) : base(gameContextManager, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.OnEnterSaveMenuState();
    }

    public override void UpdateState()
    {
        if (GameUIManager.Instance.Start)
        {
            GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.SelectSaveButton.gameObject);
        }

        if (GameUIManager.Instance.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
            {
                if (GameSaveSystem.Instance.SlotIsSelected)
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.SelectSaveButton.gameObject);
                }
                else
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.SaveSlots[0].slotButton.gameObject);
                }
            }
        }
    }

    public override void ExitState()
    {
        GameContextManager.OnExitSaveMenuState();
    }

    public override void CheckSwitchStates()
    {

    }

    public override void CheckSwitchSubStates()
    {

    }
}
