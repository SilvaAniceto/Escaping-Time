public enum ELevelState
{
    Closed,
    Open,
    Finished
}

[System.Serializable]
public class GameLevelRuntimeData
{

    public ELevelState State;
    public SceneIdentifier LevelSceneName;

    [System.NonSerialized] 
    public GameLevelConfig Config;

    public int CurrentGemScore;
    public int CurrentHourglassScore;

    public int MaxGemScoreReached;
    public int MaxHourglassScoreReached;
    public int MaxLevelScoreReached;

    public EClassficationTier ClassficationTierReached;

    public void SetClassficationTier(int currentScore)
    {
        if (currentScore < Config.Tier2TargetScore)
        {
            ClassficationTierReached = EClassficationTier.Tier1;
        }
        else if (currentScore >= Config.Tier2TargetScore && currentScore < Config.Tier3TargetScore)
        {
            ClassficationTierReached = EClassficationTier.Tier2;
        }
        else if (currentScore >= Config.Tier3TargetScore)
        {
            ClassficationTierReached = EClassficationTier.Tier3;
        }

        State = ELevelState.Finished;
    }
}
