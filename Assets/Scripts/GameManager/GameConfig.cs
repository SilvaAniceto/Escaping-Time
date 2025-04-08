using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EscapingTime_Config_Asset", menuName = "Config Asset", order = 1)]
public class GameConfig : ScriptableObject
{
    [SerializeField] private CharacterContextManager CharacterContextManager;
    [SerializeField] private List<string> _scenesList = new List<string>();

    private const string _mainMenuScene = "Assets/Scenes/MainMenu.unity";

    public string MainMenuScene {get => _mainMenuScene; }
    public List<string> SceneList { get => _scenesList; }
}
