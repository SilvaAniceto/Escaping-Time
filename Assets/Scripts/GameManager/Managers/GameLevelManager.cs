using UnityEngine;

[CreateAssetMenu(fileName = "GameLevelManager", menuName = "Game Level Manager Config", order = 2)]
[System.Serializable]
public class GameLevelManager : ScriptableObject
{
    public enum EState
    {
        Closed,
        Open,
        Finished
    }

    [Header("Level State")]
    [SerializeField] private EState _state;

    [Header("Level Name")]
    [SerializeField] private string _levelSceneName;

    [Header("Unlock Target")]
    [SerializeField] private int _levelUnlockScore;

    [Header("Level Tier Target")]
    [SerializeField] private int _tier3TargetScore;
    [SerializeField] private int _tier2TargetScore;
    [SerializeField] private int _tier1TargetScore;

    [Header("Level Max Scores")]
    [SerializeField] private int _maxGemScore;
    [SerializeField] private int _maxHourglassScore;

    [Header("Level Current Scores")]
    [SerializeField] private int _currentGemScore;
    [SerializeField] private int _currentHourglassScore;

    [SerializeField][HideInInspector] private int _maxGemScoreReached;
    [SerializeField][HideInInspector] private int _maxHourglassScoreReached;
    [SerializeField][HideInInspector] private int _maxLevelScoreReached;
    [SerializeField][HideInInspector] private EClassficationTier _classficationTierReached;

    public EState State { get => _state; set => _state = value; }
    public string LevelSceneName { get => _levelSceneName; set => _levelSceneName = value; }
    public int LevelUnlockScore { get => _levelUnlockScore; set => _levelUnlockScore = value; }
    public int Tier3TargetScore { get => _tier3TargetScore; set => _tier3TargetScore = value; }
    public int Tier2TargetScore { get => _tier2TargetScore; set => _tier2TargetScore = value; }
    public int Tier1TargetScore { get => _tier1TargetScore; set => _tier1TargetScore = value; }
    public int MaxGemScore { get => _maxGemScore; set => _maxGemScore = value; }
    public int MaxHourglassScore { get => _maxHourglassScore; set => _maxHourglassScore = value; }
    public int CurrentGemScore { get => _currentGemScore; set => _currentGemScore = value; }
    public int CurrentHourglassScore { get => _currentHourglassScore; set => _currentHourglassScore = value; }
    public int MaxGemScoreReached { get => _maxGemScoreReached; set => _maxGemScoreReached = value; }
    public int MaxHourglassScoreReached { get => _maxHourglassScoreReached; set => _maxHourglassScoreReached = value; }
    public int MaxLevelScoreReached { get => _maxLevelScoreReached; set => _maxLevelScoreReached = value; }
    public EClassficationTier ClassficationTierReached { get => _classficationTierReached; set => _classficationTierReached = value; }

    public void SetClassficationTier(int currentScore)
    {
        if (currentScore < _tier2TargetScore)
        {
            _classficationTierReached = EClassficationTier.Tier1;
        }
        else if (currentScore >= _tier2TargetScore && currentScore < _tier3TargetScore)
        {
            _classficationTierReached = EClassficationTier.Tier2;
        }
        else if (currentScore >= _tier3TargetScore)
        {
            _classficationTierReached = EClassficationTier.Tier3;
        }

        _state = EState.Finished;
    }
}
