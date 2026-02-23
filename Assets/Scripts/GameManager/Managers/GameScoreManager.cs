using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

public class GameScoreManager
{
    public static GameScoreManager Instance;

    #region PUBLIC PROPERTIES
    public GameLevelManager LevelManager { get; set; }
    public int MasterScore { get; set; }
    public int CurrentScore { get; private set; }
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
        if (Instance == null)
        {
            Instance = this;
        }

        GameContextManager = gameContextManager;

        if (!isGameContext)
        {
            LevelManager = gameContextManager.GameLevelManagers[0];
            ResetPlayerScorePoints();
        }
    }
    #endregion

    #region TIMER METHODS
    public void SetCurrentTimer()
    {
        CurrentTimer -= Time.deltaTime;
        CurrentTimer = Mathf.Clamp(CurrentTimer, 0, 300);
        GameUIManager.Instance.SetTimerDisplay(Mathf.Round(CurrentTimer).ToString());
    }
    #endregion

    public void ResetPlayerScorePoints()
    {
        CurrentScore = 0;
        CurrentTimer = 300;
        TimeScore = 0;
        LevelManager.CurrentGemScore = 0;
        LevelManager.CurrentHourglassScore = 0;

        GameUIManager.Instance.ResetScoreUI();
        GameUIManager.Instance.SetHourglassDisplay(LevelManager.CurrentHourglassScore / 100);
    }
    public void AddGemScore(int value)
    {
        LevelManager.CurrentGemScore += value;

        AddScorePoints(value);
    }
    public void AddCollectedHourglass()
    {
        LevelManager.CurrentHourglassScore += 100;

        AddScorePoints(100);

        GameUIManager.Instance.SetHourglassDisplay(LevelManager.CurrentHourglassScore / 100);
    }
    public void AddScorePoints(int value)
    {
        CurrentScore += value;

        GameUIManager.Instance.SetScoreDisplay(CurrentScore);
    }
    public void SetScoreManager()
    {
        SetFinalScore();

        SetTrophyPercentage();

        GameContextManager.StartCoroutine(SetLevelFinalScoreCoroutine());
    }
    private void SetFinalScore()
    {
        GameUIManager.Instance.SetMasterScoreText($"Master Score: {MasterScore}");

        CurrentTimer = Mathf.RoundToInt(CurrentTimer);

        TimeScore = (int)CurrentTimer * TimeScoreMultiplier;

        CurrentScore += TimeScore;

        CurrentScore = CurrentScore * (TimeScoreMultiplier == 0 ? TimeScoreMultiplier : 1);

        if (LevelManager.CurrentGemScore >= LevelManager.MaxGemScoreReached)
        {
            LevelManager.MaxGemScoreReached = LevelManager.CurrentGemScore;
        }

        if (LevelManager.CurrentHourglassScore >= LevelManager.MaxHourglassScoreReached)
        {
            LevelManager.MaxHourglassScoreReached = LevelManager.CurrentHourglassScore;
        }

        if (CurrentScore > LevelManager.MaxLevelScoreReached)
        {
            int diff = CurrentScore - LevelManager.MaxLevelScoreReached;

            LevelManager.MaxLevelScoreReached = CurrentScore;

            MasterScore += diff;
        }

        LevelManager.SetClassficationTier(CurrentScore);
    }
    private void SetTrophyPercentage()
    {
        SilverScorePercentage = Mathf.Round(Mathf.InverseLerp(0, LevelManager.Tier3TargetScore, LevelManager.Tier2TargetScore) * 100) / 100;
        BrassScorePercentage = Mathf.Round(Mathf.InverseLerp(0, LevelManager.Tier3TargetScore, LevelManager.Tier1TargetScore) * 100) / 100;

        GameUIManager.Instance.SetTrophyUIPosition();
    }
    private IEnumerator SetLevelFinalScoreCoroutine()
    {
        GameUIManager.Instance.SetTimeScoreText($"x {TimeScoreMultiplier} = {0}");

        yield return GameContextManager.StartCoroutine(SetGemUIFinalScore());
        yield return GameContextManager.StartCoroutine(SetHourglassUIFinalScore());
        yield return GameContextManager.StartCoroutine(SetTimeUIFinalScore());
        yield return GameContextManager.StartCoroutine(SetLevelUIFinalScore());

        GameUIManager.Instance.SetMasterScoreText($"Master Score: {MasterScore}");
        GameAudioManager.Instance.StopSFX();
        GameAudioManager.Instance.PlaySFX("End_Score");

        yield return new WaitForSeconds(3.00f);

        GameUIManager.Instance.ConfirmActionButton.gameObject.SetActive(true);

        GameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.ConfirmActionButton.gameObject);
    }
    private IEnumerator SetGemUIFinalScore()
    {
        GameAudioManager.Instance.StopSFX();
        GameAudioManager.Instance.PlaySFX("Level_Score");

        float gemUIScore = 0;

        while (gemUIScore < LevelManager.CurrentGemScore)
        {
            gemUIScore += Time.deltaTime / 3;

            gemUIScore = Mathf.CeilToInt(gemUIScore);

            gemUIScore = Mathf.Clamp(gemUIScore, 0, LevelManager.CurrentGemScore);

            GameUIManager.Instance.SetGemScoreText($"= {gemUIScore.ToString()}");

            yield return null;
        }

        GameAudioManager.Instance.StopSFX();
    }
    private IEnumerator SetHourglassUIFinalScore()
    {
        GameAudioManager.Instance.PlaySFX("Level_Score");

        float hourglassUIScore = 0;

        while (hourglassUIScore < LevelManager.CurrentHourglassScore)
        {
            hourglassUIScore +=  Time.deltaTime / 3;

            hourglassUIScore = Mathf.CeilToInt(hourglassUIScore);

            hourglassUIScore = Mathf.Clamp(hourglassUIScore, 0, LevelManager.CurrentGemScore);

            GameUIManager.Instance.SetHourglassText($"= {hourglassUIScore}");

            yield return null;
        }

        GameAudioManager.Instance.StopSFX();
    }
    private IEnumerator SetTimeUIFinalScore()
    {
        GameAudioManager.Instance.PlaySFX("Level_Score");

        float timeUIScore = 0;

        while (timeUIScore < TimeScore)
        {
            timeUIScore += Time.deltaTime;

            timeUIScore = Mathf.CeilToInt(timeUIScore);

            timeUIScore = Mathf.Clamp(timeUIScore, 0, TimeScore);

            GameUIManager.Instance.SetTimeScoreText($"x {TimeScoreMultiplier} = {timeUIScore}");

            yield return null;
        }

        GameAudioManager.Instance.StopSFX();
    }
    private IEnumerator SetLevelUIFinalScore()
    {
        GameAudioManager.Instance.PlaySFX("Final_Score");

        float levelUIFinalScore = 0;

        float finalScorePercentage = Mathf.InverseLerp(0.00f, LevelManager.Tier3TargetScore, CurrentScore);

        while (levelUIFinalScore < finalScorePercentage)
        {
            levelUIFinalScore += Time.deltaTime / 3;

            levelUIFinalScore = Mathf.Clamp(levelUIFinalScore, 0.00f, finalScorePercentage);

            GameUIManager.Instance.SetFillAmount(levelUIFinalScore);

            GameAudioManager.Instance.LerpPitch("Final_Score", levelUIFinalScore);

            if (levelUIFinalScore >= BrassScorePercentage)
            {
                GameUIManager.Instance.ResetBrassTrophy();
            }
            if (levelUIFinalScore >= SilverScorePercentage)
            {
                GameUIManager.Instance.ResetSilverTrophy();
            }
            if (levelUIFinalScore >= 1)
            {
                GameUIManager.Instance.ResetGoldTrophy();
            }

            yield return null;
        }
    }
}