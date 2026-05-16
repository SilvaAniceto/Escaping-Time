using UnityEngine.Events;

public static class GameEventsManager
{
    public static UnityEvent<bool> OnPauseStateChanged = new UnityEvent<bool>();

    public static UnityEvent<SceneIdentifier> OnSceneLoadRequested = new UnityEvent<SceneIdentifier>();
    public static UnityEvent OnSceneLoaded = new UnityEvent();

    public static UnityEvent OnHubEntered = new UnityEvent();

    public static UnityEvent<int> OnScoreUpdated = new UnityEvent<int>();
    public static UnityEvent<string> OnTimerUpdated = new UnityEvent<string>();
    public static UnityEvent<int> OnHourglassUpdated = new UnityEvent<int>();
    public static UnityEvent<string> OnMasterScoreUpdated = new UnityEvent<string>();

    public static UnityEvent OnScorePanelShown = new UnityEvent();
    public static UnityEvent OnScorePanelHidden = new UnityEvent();
    public static UnityEvent<string> OnGemScoreTextUpdated = new UnityEvent<string>();
    public static UnityEvent<string> OnHourglassTextUpdated = new UnityEvent<string>();
    public static UnityEvent<string> OnTimeScoreTextUpdated = new UnityEvent<string>();
    public static UnityEvent<float> OnScoreFillAmountUpdated = new UnityEvent<float>();
    public static UnityEvent OnBrassTrophyReset = new UnityEvent();
    public static UnityEvent OnSilverTrophyReset = new UnityEvent();
    public static UnityEvent OnGoldTrophyReset = new UnityEvent();
    public static UnityEvent OnConfirmActionShown = new UnityEvent();
    public static UnityEvent OnConfirmActionHidden = new UnityEvent();
    public static UnityEvent OnConfirmActionSelected = new UnityEvent();

    public static UnityEvent OnLevelStarted = new UnityEvent();
    public static UnityEvent OnLevelFinished = new UnityEvent();

    public static UnityEvent OnCharacterReset = new UnityEvent();
}
