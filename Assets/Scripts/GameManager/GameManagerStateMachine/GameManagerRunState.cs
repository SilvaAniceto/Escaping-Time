using UnityEngine;

public class GameManagerRunState : GameManagerAbstractState
{
    public GameManagerRunState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory) : base(gameManagerContext, gameManagerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameManagerContext.OnRunOrPauseStateChanged?.Invoke(true);

        GameManagerContext.CharacterUI.SetActive(true);
    }

    public override void UpdateState()
    {
       
    }

    public override void ExitState()
    {
        GameManagerContext.CharacterUI.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (GameManagerContext.PauseInput)
        {
            SwitchState(GameManagerStateFactory.GamePauseState());
        }
    }

    public override void CheckSwitchSubStates()
    {
        
    }
}
