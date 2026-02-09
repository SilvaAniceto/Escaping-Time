using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class GameManagerRunState : GameManagerAbstractState
{
    public GameManagerRunState(GameContextManager gameContextManager, GameManagerStateFactory gameManagerStateFactory, GameUIManager gameUIInputsManager) : base(gameContextManager, gameManagerStateFactory, gameUIInputsManager)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        GameContextManager.ExitState = null;

        GameContextManager.GameUIManager.SetLevelUIObjects();

        GameContextManager.GameUIManager.CharacterUIManager.gameObject.SetActive(true);

        GameContextManager.GameUIManager.SetScoreDisplay(GameContextManager.ScoreManager.CurrentScore);

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(null);
    }

    public override void UpdateState()
    {
       
    }

    public override void ExitState()
    {
        GameContextManager.GameUIManager.CharacterUIManager.gameObject.SetActive(false);

        GameContextManager.SetLevelScore = false;
    }

    public override void CheckSwitchStates()
    {
        if (GameContextManager.PlayerInputManager.Cancel)
        {
            GameContextManager.ExitState = GameManagerStateFactory.GameRunState();

            GameContextManager.GameUIManager.ExitLevelButtonText.text = "Back to Hub";
            GameContextManager.GameUIManager.ConfirmPanelText.text = "Quit to Hub?";

            GameContextManager.GameUIManager.ExitLevelButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.ExitLevelButton.onClick.AddListener(() =>
            {
                GameContextManager.GameAudioManager.PlaySFX("Menu_Click");

                System.Action action = () =>
                {
                    GameContextManager.GameUIManager.ConfirmPanel.SetActive(true);
                    GameContextManager.GameUIManager.PauseMenu.SetActive(false);
                    GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameContextManager.GameUIManager.ConfirmMainMenuButton.gameObject);
                };

                GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Click"));
            });

            GameContextManager.GameUIManager.ConfirmMainMenuButton.onClick.RemoveAllListeners();
            GameContextManager.GameUIManager.ConfirmMainMenuButton.onClick.AddListener(() =>
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
                    GameContextManager.CharacterContextManager.transform.position = GameContextManager.CharacterHubStartPosition;
                });

                GameContextManager.GameAudioManager.PlaySFX("Menu_Start");
                GameContextManager.TargetScene = "Level_Hub";

                System.Action action = () =>
                {
                    GameContextManager.ExitState = GameManagerStateFactory.GameHubState();
                    GameContextManager.GameUIManager.ConfirmPanel.SetActive(false);
                    SwitchState(GameManagerStateFactory.GameLoadingState());
                };

                GameContextManager.GameAudioManager.StopFadedBGM(0.00f, 1.00f);
                GameContextManager.WaitSeconds(action, GameContextManager.GameAudioManager.AudioClipLength("Menu_Start"));
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
