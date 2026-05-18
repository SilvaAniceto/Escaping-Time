using UnityEngine;

[System.Serializable]
public class GameEnvironmentManager
{
    public enum Environment
    {
        Development,
        GameContext
    }

    [Header("Environment Settings")]
    [SerializeField] private Environment _environment = Environment.GameContext;

    public Environment CurrentEnvironment => _environment;

    public void ApplyGameEnvironmentSettings()
    {
#if !UNITY_EDITOR
        _environment = Environment.GameContext;
        Screen.SetResolution(1920, 1080, true);
        Application.targetFrameRate = 60;
#endif
    }
}
