using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoader : MonoBehaviour
{
    [Header("Loading Screen")]
    [SerializeField] private GameObject _loadingScreen;

    public SceneIdentifier TargetScene { get; set; }

    public void InitializeGameSceneLoader()
    {
        GameEventsManager.OnTargetSceneUpdated.AddListener(UpdateTargetScene);
    }

    public void UpdateTargetScene(SceneIdentifier sceneId)
    {
        TargetScene = sceneId;
    }

    public void LoadTargetScene()
    {
        SceneManager.LoadSceneAsync(TargetScene.ToString());
    }

    private void OnDestroy()
    {
        GameEventsManager.OnTargetSceneUpdated.RemoveListener(UpdateTargetScene);
    }
}
