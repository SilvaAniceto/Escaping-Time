using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

public class GameScoreManager : MonoBehaviour
{
    #region INSPECTOR FIELDS
    [Header("Image UI Objects")]
    [SerializeField] private Image _fill;
    [SerializeField] private Image _brass;
    [SerializeField] private Image _silver;
    [SerializeField] private Image _gold;

    [Header("Score UI Texts")]
    [SerializeField] private Text _gemScore;
    [SerializeField] private Text _hourglassScore;
    [SerializeField] private Text _timeScore;
    [SerializeField] private Text _masterScore;
    #endregion

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
    private float GemFinalScore { get; set; } = 0;
    private float HourglassFinalScore { get; set; } = 0;
    private float TimeFinalScore { get; set; } = 0;
    private float LevelFinalScore { get; set; } = 0;
    private float SilverScorePercentage { get; set; } = 0;
    private float BrassScorePercentage { get; set; } = 0;
    private float FinalScorePercentage { get; set; } = 0; 
    #endregion

    #region PRIVATE CALLBACK
    private UnityEvent OnLevelFinalScore = new UnityEvent();
    #endregion

    #region DEFAULT METHODS
    public void Initialize(GameContextManager gameContextManager, bool isGameContext = true)
    {
        GameContextManager = gameContextManager;

        if (!isGameContext)
        {
            LevelManager = GameContextManager.GameLevelManagers[0];
            ResetPlayerScorePoints();
        }

    }
    private void Update()
    {
        OnLevelFinalScore?.Invoke();
    }
    #endregion

    #region TIMER METHODS
    public void SetCurrentTimer()
    {
        CurrentTimer -= Time.deltaTime;
        CurrentTimer = Mathf.Clamp(CurrentTimer, 0, 300);
        GameContextManager.GameUIManager.CharacterUIManager.SetTimerDisplay(Mathf.Round(CurrentTimer).ToString());
    }
    #endregion

    public void ResetPlayerScorePoints()
    {
        CurrentScore = 0;
        CurrentTimer = 300;
        TimeScore = 0;
        LevelManager.CurrentGemScore = 0;
        LevelManager.CurrentHourglassScore = 0;

        _brass.color = Color.black;
        _silver.color = Color.black;
        _gold.color = Color.black;
        _fill.fillAmount = 0;

        GameContextManager.GameUIManager.CharacterUIManager.SetHourglassDisplay(LevelManager.CurrentHourglassScore / 100);
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

        GameContextManager.GameUIManager.CharacterUIManager.SetHourglassDisplay(LevelManager.CurrentHourglassScore / 100);
    }
    public void AddScorePoints(int value)
    {
        CurrentScore += value;

        GameContextManager.GameUIManager.CharacterUIManager.SetScoreDisplay(CurrentScore);
    }
    public void SetFinalScore()
    {
        _masterScore.text = $"Master Score: {MasterScore}";

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

        GameContextManager.SetLevelScore = true;
    }
    public void SetScoreUITexts()
    {
        SetTrophyPercentagePosition();

        StartCoroutine(SetLevelFinalScoreUI());
    }
    private void SetTrophyPercentagePosition()
    {
        SilverScorePercentage = Mathf.Round(Mathf.InverseLerp(0, LevelManager.Tier3TargetScore, LevelManager.Tier2TargetScore) * 100) / 100;
        BrassScorePercentage = Mathf.Round(Mathf.InverseLerp(0, LevelManager.Tier3TargetScore, LevelManager.Tier1TargetScore) * 100) / 100;

        _silver.rectTransform.anchoredPosition = new Vector2(_fill.rectTransform.rect.width * SilverScorePercentage, _silver.rectTransform.anchoredPosition.y);
        _brass.rectTransform.anchoredPosition = new Vector2(_fill.rectTransform.rect.width * BrassScorePercentage, _brass.rectTransform.anchoredPosition.y);
    }
    IEnumerator SetLevelFinalScoreUI()
    {
        GameContextManager.FinishSetLevelScore = false;

        _timeScore.text = $"x {TimeScoreMultiplier} = {0}";

        GemFinalScore = 0;
        HourglassFinalScore = 0;
        TimeFinalScore = 0;
        LevelFinalScore = 0;

        FinalScorePercentage = Mathf.InverseLerp(0.00f, LevelManager.Tier3TargetScore, CurrentScore);

        OnLevelFinalScore.RemoveAllListeners();
        OnLevelFinalScore.AddListener(SetGemFinalScore);
        GameContextManager.GameAudioManager.StopSFX();

        yield return new WaitUntil(() => GemFinalScore >=  LevelManager.CurrentGemScore);  

        OnLevelFinalScore.RemoveAllListeners();
        OnLevelFinalScore.AddListener(SetHourglassFinalScore);
        GameContextManager.GameAudioManager.StopSFX();

        yield return new WaitUntil(() => HourglassFinalScore >= LevelManager.CurrentHourglassScore);

        OnLevelFinalScore.RemoveAllListeners();
        OnLevelFinalScore.AddListener(SetTimeFinalScore);
        GameContextManager.GameAudioManager.StopSFX();

        yield return new WaitUntil(() => TimeFinalScore >= Mathf.RoundToInt(CurrentTimer * TimeScoreMultiplier));

        OnLevelFinalScore.RemoveAllListeners();
        OnLevelFinalScore.AddListener(SetLevelFinalScore);
        GameContextManager.GameAudioManager.StopSFX();

        yield return new WaitUntil(() => LevelFinalScore >= FinalScorePercentage);

        OnLevelFinalScore.RemoveAllListeners();
        _masterScore.text = $"Master Score: {MasterScore}";
        GameContextManager.GameAudioManager.StopSFX();
        GameContextManager.GameAudioManager.PlaySFX("End_Score");

        yield return new WaitForSeconds(3.00f);

        GameContextManager.FinishSetLevelScore = true;
    }
    private void SetGemFinalScore()
    {
        GameContextManager.GameAudioManager.PlaySFX("Level_Score");

        GemFinalScore += 1.5f;

        GemFinalScore = Mathf.RoundToInt(GemFinalScore);

        GemFinalScore = Mathf.Clamp(GemFinalScore, 0, LevelManager.CurrentGemScore);

        _gemScore.text = $"= {GemFinalScore}";
    }
    private void SetHourglassFinalScore()
    {
        GameContextManager.GameAudioManager.PlaySFX("Level_Score");

        HourglassFinalScore += 0.8f;

        HourglassFinalScore = Mathf.RoundToInt(HourglassFinalScore);

        HourglassFinalScore = Mathf.Clamp(HourglassFinalScore, 0, LevelManager.CurrentHourglassScore);

        _hourglassScore.text = $"= {HourglassFinalScore}";
    }
    private void SetTimeFinalScore()
    {
        GameContextManager.GameAudioManager.PlaySFX("Level_Score");

        TimeFinalScore += 2.6f;

        TimeFinalScore = Mathf.RoundToInt(TimeFinalScore);

        TimeFinalScore = Mathf.Clamp(TimeFinalScore, 0, TimeScore);

        _timeScore.text = $"x {TimeScoreMultiplier} = {TimeFinalScore}";
    }
    private void SetLevelFinalScore()
    {
        LevelFinalScore += 0.002f;

        LevelFinalScore = Mathf.Clamp(LevelFinalScore, 0.00f, FinalScorePercentage);

        _fill.fillAmount = LevelFinalScore;

        GameContextManager.GameAudioManager.PlaySFX("Final_Score");
        GameContextManager.GameAudioManager.LerpPitch("Final_Score", LevelFinalScore);

        if (LevelFinalScore >= BrassScorePercentage)
        {
            _brass.color = Color.white;
        }
        if (LevelFinalScore >= SilverScorePercentage)
        {
            _silver.color = Color.white;
        }
        if (LevelFinalScore >= 1)
        {
            _gold.color = Color.white;
        }
    }
}