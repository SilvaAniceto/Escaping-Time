public class GameManagerLoadingState : GameManagerAbstractState
{
    public GameManagerLoadingState(GameManagerContext gameManagerContext, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameManagerContext, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (GameManagerContext.CharacterContextManager)
        {
            GameManagerContext.CharacterContextManager.DisableCharacter();
        }

        GameManagerContext.CharacterUI.gameObject.SetActive(false);
        GameManagerContext.LoadingScreen.SetActive(true);

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GameManagerContext.TargetScene);

        GameManagerContext.OnLoadSceneEnd.RemoveAllListeners();
        GameManagerContext.OnLoadSceneEnd.AddListener(() =>
        {
            SwitchState(GameManagerContext.ExitState);
        });

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += GameManagerContext.AfterLoadSceneEnd;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= GameManagerContext.AfterLoadSceneEnd;
        GameManagerContext.LoadingScreen.SetActive(false);
    }

    public override void CheckSwitchStates()
    {

    }

    public override void CheckSwitchSubStates()
    {

    }
}
