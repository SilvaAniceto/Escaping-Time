using System.Collections;
using UnityEngine;
public enum ETierScore
{
    Tier1 = 10, // 60  seconds - 300
    Tier2 = 20, // 120 seconds - 600
    Tier3 = 30, // 180 seconds - 900
    Tier4 = 40, // 240 seconds - 1200
    Tier5 = 50  // 300 seconds - 1500
}

public enum EClassficationTier
{
    None = 0,
    Tier1 = 1,
    Tier2 = 2, 
    Tier3 = 3
}

public class GameScoreManager : IScoreManager
{
    #region PUBLIC PROPERTIES
    public GameLevelRuntimeData CurrentLevelRuntimeData { get; set; }
    public int MasterScore { get; set; }
    public int CurrentScore { get; set; }
    #endregion

    #region PRIVATE PROPERTIES
    private GameContextManager GameContextManager { get; set; }
    private float CurrentTimer { get; set; } = 300;
    private int TimeScoreMultiplier
    {
        get
        {
            if (CurrentTimer == 0)
            {
                return 0;
            }
            else if (CurrentTimer <= 60)
            {
                return 1;
            }
            else if (CurrentTimer <= 120)
            {
                return 2;
            }
            else if (CurrentTimer <= 150)
            {
                return 3;
            }
            return 5;
        }
    }
    private int TimeScore { get; set; }
    public float SilverScorePercentage { get; set; } = 0;
    public float BrassScorePercentage { get; set; } = 0; 
    #endregion

    #region DEFAULT METHODS
    public void Initialize(GameContextManager gameContextManager, bool isGameContext = true)
    {
        GameContextManager = gameContextManager;

        if (!isGameContext)
        {
            CurrentLevelRuntimeData = gameContextManager.GameLevelsRuntimeData[0];
            ResetPlayerScorePoints();
        }
    }
    #endregion

    #region TIMER METHODS
    public void SetCurrentTimer()
    {
        CurrentTimer -= Time.deltaTime;
        CurrentTimer = Mathf.Clamp(CurrentTimer, 0, 300);
        GameEventsManager.OnTimerUpdated?.Invoke(Mathf.Round(CurrentTimer).ToString());
    }
    #endregion

    public void ResetPlayerScorePoints()
    {
        CurrentScore = 0;
        CurrentTimer = 300;
        TimeScore = 0;
        CurrentLevelRuntimeData.CurrentGemScore = 0;
        CurrentLevelRuntimeData.CurrentHourglassScore = 0;

        GameEventsManager.OnScoreUpdated?.Invoke(0);
        GameEventsManager.OnHourglassUpdated?.Invoke(0);
    }
    public void AddGemScore(int value)
    {
        CurrentLevelRuntimeData.CurrentGemScore += value;

        AddScorePoints(value);
    }
    public void AddCollectedHourglass()
    {
        CurrentLevelRuntimeData.CurrentHourglassScore += 100;

        AddScorePoints(100);

        GameEventsManager.OnHourglassUpdated?.Invoke(CurrentLevelRuntimeData.CurrentHourglassScore / 100);
    }
    public void AddScorePoints(int value)
    {
        CurrentScore += value;

        GameEventsManager.OnScoreUpdated?.Invoke(CurrentScore);
    }
    public void SetScoreManager()
    {
        SetFinalScore();

        SetTrophyPercentage();

        GameEventsManager.OnScorePanelShown?.Invoke();

        GameContextManager.StartCoroutine(SetLevelFinalScoreCoroutine());
    }
    private void SetFinalScore()
    {
        GameEventsManager.OnMasterScoreUpdated?.Invoke($"Master Score: {MasterScore}");

        CurrentTimer = Mathf.RoundToInt(CurrentTimer);

        TimeScore = (int)CurrentTimer * TimeScoreMultiplier;

        CurrentScore += TimeScore;

        CurrentScore = CurrentScore * (TimeScoreMultiplier == 0 ? TimeScoreMultiplier : 1);

        if (CurrentLevelRuntimeData.CurrentGemScore >= CurrentLevelRuntimeData.MaxGemScoreReached)
        {
            CurrentLevelRuntimeData.MaxGemScoreReached = CurrentLevelRuntimeData.CurrentGemScore;
        }

        if (CurrentLevelRuntimeData.CurrentHourglassScore >= CurrentLevelRuntimeData.MaxHourglassScoreReached)
        {
            CurrentLevelRuntimeData.MaxHourglassScoreReached = CurrentLevelRuntimeData.CurrentHourglassScore;
        }

        if (CurrentScore > CurrentLevelRuntimeData.MaxLevelScoreReached)
        {
            int diff = CurrentScore - CurrentLevelRuntimeData.MaxLevelScoreReached;

            CurrentLevelRuntimeData.MaxLevelScoreReached = CurrentScore;

            MasterScore += diff;
        }

        CurrentLevelRuntimeData.SetClassficationTier(CurrentScore);
    }
    private void SetTrophyPercentage()
    {
        SilverScorePercentage = Mathf.Round(Mathf.InverseLerp(0, CurrentLevelRuntimeData.Config.Tier3TargetScore, CurrentLevelRuntimeData.Config.Tier2TargetScore) * 100) / 100;
        BrassScorePercentage = Mathf.Round(Mathf.InverseLerp(0, CurrentLevelRuntimeData.Config.Tier3TargetScore, CurrentLevelRuntimeData.Config.Tier1TargetScore) * 100) / 100;

        GameContextManager.UIManager.SetTrophyUIPosition();
    }
    private IEnumerator SetLevelFinalScoreCoroutine()
    {
        GameEventsManager.OnTimeScoreTextUpdated?.Invoke($"x {TimeScoreMultiplier} = {0}");

        yield return GameContextManager.StartCoroutine(SetGemUIFinalScore());
        yield return GameContextManager.StartCoroutine(SetHourglassUIFinalScore());
        yield return GameContextManager.StartCoroutine(SetTimeUIFinalScore());
        yield return GameContextManager.StartCoroutine(SetLevelUIFinalScore());

        GameEventsManager.OnMasterScoreUpdated?.Invoke($"Master Score: {MasterScore}");
        GameContextManager.AudioManager.StopSFX();
        GameContextManager.AudioManager.PlaySFX("End_Score");

        yield return new WaitForSeconds(3.00f);

        GameEventsManager.OnConfirmActionShown?.Invoke();

        GameEventsManager.OnConfirmActionSelected?.Invoke();
    }
    private IEnumerator SetGemUIFinalScore()
    {
        GameContextManager.AudioManager.StopSFX();
        GameContextManager.AudioManager.PlaySFX("Level_Score");

        float gemUIScore = 0;

        while (gemUIScore < CurrentLevelRuntimeData.CurrentGemScore)
        {
            gemUIScore += Time.deltaTime / 3;

            gemUIScore = Mathf.CeilToInt(gemUIScore);

            gemUIScore = Mathf.Clamp(gemUIScore, 0, CurrentLevelRuntimeData.CurrentGemScore);

            GameEventsManager.OnGemScoreTextUpdated?.Invoke($"= {gemUIScore.ToString()}");

            yield return null;
        }

        GameContextManager.AudioManager.StopSFX();
    }
    private IEnumerator SetHourglassUIFinalScore()
    {
        GameContextManager.AudioManager.PlaySFX("Level_Score");

        float hourglassUIScore = 0;

        while (hourglassUIScore < CurrentLevelRuntimeData.CurrentHourglassScore)
        {
            hourglassUIScore +=  Time.deltaTime / 3;

            hourglassUIScore = Mathf.CeilToInt(hourglassUIScore);

            hourglassUIScore = Mathf.Clamp(hourglassUIScore, 0, CurrentLevelRuntimeData.CurrentGemScore);

            GameEventsManager.OnHourglassTextUpdated?.Invoke($"= {hourglassUIScore.ToString()}");

            yield return null;
        }

        GameContextManager.AudioManager.StopSFX();
    }
    private IEnumerator SetTimeUIFinalScore()
    {
        GameContextManager.AudioManager.PlaySFX("Level_Score");

        float timeUIScore = 0;

        while (timeUIScore < TimeScore)
        {
            timeUIScore += Time.deltaTime;

            timeUIScore = Mathf.CeilToInt(timeUIScore);

            timeUIScore = Mathf.Clamp(timeUIScore, 0, TimeScore);

            GameEventsManager.OnTimeScoreTextUpdated?.Invoke($"x {TimeScoreMultiplier} = {timeUIScore}");

            yield return null;
        }

        GameContextManager.AudioManager.StopSFX();
    }
    private IEnumerator SetLevelUIFinalScore()
    {
        GameContextManager.AudioManager.PlaySFX("Final_Score");

        float levelUIFinalScore = 0;

        float finalScorePercentage = Mathf.InverseLerp(0.00f, CurrentLevelRuntimeData.Config.Tier3TargetScore, CurrentScore);

        while (levelUIFinalScore < finalScorePercentage)
        {
            levelUIFinalScore += Time.deltaTime / 3;

            levelUIFinalScore = Mathf.Clamp(levelUIFinalScore, 0.00f, finalScorePercentage);

            GameEventsManager.OnScoreFillAmountUpdated?.Invoke(levelUIFinalScore);

            GameContextManager.AudioManager.LerpPitch("Final_Score", levelUIFinalScore);

            if (levelUIFinalScore >= BrassScorePercentage)
            {
                GameEventsManager.OnBrassTrophyReset?.Invoke();
            }
            if (levelUIFinalScore >= SilverScorePercentage)
            {
                GameEventsManager.OnSilverTrophyReset?.Invoke();
            }
            if (levelUIFinalScore >= 1)
            {
                GameEventsManager.OnGoldTrophyReset?.Invoke();
            }

            yield return null;
        }
    }
}