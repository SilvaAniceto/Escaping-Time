using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManagerRunState : GameManagerAbstractState
{
    public GameManagerRunState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIInputsManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.ExitState = null;

        GameContextManager.CharacterUI.SetLevelUIObjects();

        GameContextManager.CharacterUI.gameObject.SetActive(true);

        GameContextManager.CharacterUI.SetScoreDisplay(GameContextManager.ScoreManager.CurrentScore);

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(null);
    }

    public override void UpdateState()
    {
       
    }

    public override void ExitState()
    {
        GameContextManager.CharacterUI.gameObject.SetActive(false);

        GameContextManager.SetLevelScore = false;
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.CharacterContextManager.CurrentState.PlayerInputManager.Cancel)
        {
            GameContextManager.ExitState = GameManagerStateFactory.GameRunState();

            GameContextManager.ExitLevelButtonText.text = "Back to Hub";
            GameContextManager.ConfirmPanelText.text = "Quit to Hub?";

            GameContextManager.ExitLevelButton.onClick.RemoveAllListeners();
            GameContextManager.ExitLevelButton.onClick.AddListener(() =>
            {
                GameAudioManager.Instance.PlaySFX("Menu_Click");

                System.Action action = () =>
                {
                    GameContextManager.ConfirmPanel.SetActive(true);
                    GameContextManager.PauseMenu.SetActive(false);
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.ConfirmMainMenuButton.gameObject);
                };

                GameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Click"));
            });

            GameContextManager.ConfirmMainMenuButton.onClick.RemoveAllListeners();
            GameContextManager.ConfirmMainMenuButton.onClick.AddListener(() =>
            {
                GameContextManager.CharacterContextManager.enabled = true;
                GameContextManager.CharacterContextManager.DisableCharacterContext();

                GameStateTransitionManager.OnFadeInEnd.AddListener(() =>
                {
                    GameContextManager.CharacterContextManager.EnableCharacterContext();
                });

                GameStateTransitionManager.OnFadeInStart.AddListener(() =>
                {
                    GameContextManager.CharacterContextManager.CurrentState.CharacterAnimationManager.SetIdleAnimation();
                    GameContextManager.CharacterContextManager.transform.position = GameContextManager.Instance.CharacterHubStartPosition;
                });

                GameAudioManager.Instance.PlaySFX("Menu_Start");
                GameContextManager.TargetScene = "Level_Hub";

                System.Action action = () =>
                {
                    GameContextManager.ExitState = GameManagerStateFactory.GameHubState();
                    GameContextManager.ConfirmPanel.SetActive(false);
                    SwitchState(GameManagerStateFactory.GameLoadingState());
                };

                GameAudioManager.Instance.StopFadedBGM(0.00f, 1.00f);
                GameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Start"));
            });

            SwitchState(GameManagerStateFactory.GamePauseState());
        }

        if (GameContextManager.SetLevelScore)
        {
            GameContextManager.ExitState = GameManagerStateFactory.GameHubState();

            SwitchState(GameManagerStateFactory.GameScoreState());
        }
    }

    public override void CheckSwitchSubStates()
    {
        
    }
}
