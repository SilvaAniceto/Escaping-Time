using System.Diagnostics;

public class GameManagerLoadingState : GameManagerAbstractState
{
    public GameManagerLoadingState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameStateTransitionManager.FadeOff();

        GameContextManager.CharacterUI.gameObject.SetActive(false);
        GameContextManager.LoadingScreen.SetActive(true);

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(GameContextManager.TargetScene);

        GameContextManager.OnLoadSceneEnd.RemoveAllListeners();
        GameContextManager.OnLoadSceneEnd.AddListener(() =>
        {
            GameContextManager.WaitFrameEnd(() =>
            {
                SwitchState(GameContextManager.ExitState);
            });
        });

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += GameContextManager.AfterLoadSceneEnd;

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(null);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= GameContextManager.AfterLoadSceneEnd;
        GameContextManager.LoadingScreen.SetActive(false);
        GameStateTransitionManager.FadeIn();
    }

    public override void CheckSwitchStates()
    {

    }

    public override void CheckSwitchSubStates()
    {

    }
}
