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
        if (GameContextManager.UIManager.Start)
        {
            GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.UIManager.SelectSaveButton.gameObject);
        }

        if (GameContextManager.UIManager.Navigating)
        {
            if (GameContextManager.GameManagerEventSystem.currentSelectedGameObject == null)
            {
                if (GameContextManager.SaveSystem.SlotIsSelected)
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.UIManager.SelectSaveButton.gameObject);
                }
                else
                {
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.UIManager.SaveSlots[0].slotButton.gameObject);
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
