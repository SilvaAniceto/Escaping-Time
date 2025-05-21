using UnityEngine;

[CreateAssetMenu(fileName = "PlayeableCharacterSet", menuName = "PlayeableCharacterSet Asset", order = 1)]
public class PlayeableCharacterSet : ScriptableObject
{
    [SerializeField] private CharacterContextManager _characterContextManager;
    [SerializeField] private CameraBehaviourController _cameraBehaviourController;
    [SerializeField] private string _defaultScene;
    [SerializeField] private string _mainMenuScene;

    public CharacterContextManager CharacterContextManager { get => _characterContextManager; }
    public CameraBehaviourController CameraBehaviourController { get => _cameraBehaviourController; }
    public string DefaultScene { get => _defaultScene; }
    public string MainMenuScene {get => _mainMenuScene; }
}
