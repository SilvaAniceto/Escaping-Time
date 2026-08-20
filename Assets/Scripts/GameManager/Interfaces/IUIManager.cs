using UnityEngine;

public interface IUIManager
{
    bool Navigating { get; }
    bool Confirm { get; }
    bool Start { get; }
    void SetConfirmAction();
    void SetScoreDisplay(int points);
    void SetTimerDisplay(string time);
    void SetAirJumpPowerUpUI(string clip);
    void SetDashPowerUpUI(string clip);
    void SetWallMovePowerUpUI(string clip);
    void SetHourglassDisplay(int count);
    void SetOvertimeAirJumpPowerUpUI(float value, CharacterContextManager characterContextManager);
    void SetOvertimeDashPowerUpUI(float value, CharacterContextManager characterContextManager);
    void SetOvertimeWallMovePowerUpUI(float value, CharacterContextManager characterContextManager);
    void SetHubUIObjects();
    void SetLevelUIObjects();
    void SetTrophyUIPosition();
    void SetGemScoreText(string text);
    void SetHourglassText(string text);
    void SetTimeScoreText(string text);
    void SetMasterScoreText(string text);
    void SetFillAmount(float value);
    void ResetBrassTrophy();
    void ResetSilverTrophy();
    void ResetGoldTrophy();
    void ResetScoreUI();
    void ShowScorePanel();
    void HideScorePanel();
}