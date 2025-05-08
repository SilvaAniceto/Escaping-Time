using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameConfig_Asset", menuName = "GameConfig Asset", order = 1)]
public class GameConfig : ScriptableObject
{
    [SerializeField] private CharacterContextManager _characterContextManager;
    [SerializeField] private string _defaultScene;
    [SerializeField] private string _mainMenuScene;
    [SerializeField] private List<string> _scenesList = new List<string>();

    public CharacterContextManager CharacterContextManager { get => _characterContextManager; }
    public string DefaultScene { get => _defaultScene; }
    public string MainMenuScene {get => _mainMenuScene; }
    public List<string> SceneList { get => _scenesList; }
}
