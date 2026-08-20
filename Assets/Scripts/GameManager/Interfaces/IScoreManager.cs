public interface IScoreManager
{
    GameLevelRuntimeData CurrentLevelRuntimeData { get; set; }
    int MasterScore { get; set; }
    int CurrentScore { get; set; }
    void AddGemScore(int value);
    void AddCollectedHourglass();
    void ResetPlayerScorePoints();
    void SetScoreManager();
}