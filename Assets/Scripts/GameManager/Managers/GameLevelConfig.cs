using UnityEngine;

[CreateAssetMenu(fileName = "GameLevelConfig", menuName = "Game Level Config", order = 2)]
public class GameLevelConfig : ScriptableObject
{
    [Header("Level Name")]
    [SerializeField] private SceneIdentifier _levelSceneName;

    [Header("Unlock Target")]
    [SerializeField] private int _levelUnlockScore;

    [Header("Level Tier Target")]
    [SerializeField] private int _tier3TargetScore;
    [SerializeField] private int _tier2TargetScore;
    [SerializeField] private int _tier1TargetScore;

    [Header("Level Max Scores")]
    [SerializeField] private int _maxGemScore;
    [SerializeField] private int _maxHourglassScore;

    public SceneIdentifier LevelSceneName { get => _levelSceneName; set => _levelSceneName = value; }
    public int LevelUnlockScore { get => _levelUnlockScore; set => _levelUnlockScore = value; }
    public int Tier3TargetScore { get => _tier3TargetScore; set => _tier3TargetScore = value; }
    public int Tier2TargetScore { get => _tier2TargetScore; set => _tier2TargetScore = value; }
    public int Tier1TargetScore { get => _tier1TargetScore; set => _tier1TargetScore = value; }
    public int MaxGemScore { get => _maxGemScore; set => _maxGemScore = value; }
    public int MaxHourglassScore { get => _maxHourglassScore; set => _maxHourglassScore = value; }
}
