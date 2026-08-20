using System.Collections.Generic;
using UnityEngine;

public interface IGameFlowManager
{
    bool SetTimer { get; set; }
    bool LoadLevel { get; set; }
    CharacterContextManager CharacterContextManager { get; }
    Vector2 CharacterHubStartPosition { get; set; }
    List<GameLevelRuntimeData> GameLevelsRuntimeData { get; }
    void StartScoreState();
    void QuitToMainMenu();
    void OnQuitToMainMenu();
}