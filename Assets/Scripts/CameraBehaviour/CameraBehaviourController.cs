using Unity.Cinemachine;
using UnityEngine;

public class CameraBehaviourController : MonoBehaviour
{
    [SerializeField] private CinemachinePositionComposer _cameraPositionComposer;

    private float _cameraVerticalOffset;

    private void Awake()
    {
        GameManagerContext.OnRunOrPauseStateChanged.AddListener((value) => { this.enabled = value; });
    }

    private void Update()
    {
        CameraVerticalOffset(PlayerInputManager.PlayerInputInstance.CameraTiltInput);
    }

    private void OnDestroy()
    {
        
    }

    public void CameraVerticalOffset(float input)
    {
        _cameraVerticalOffset += Time.deltaTime * input;

        if (input == 0)
        {
            _cameraVerticalOffset = Mathf.Lerp(_cameraVerticalOffset, 0.00f, Time.deltaTime * 22.00f);
        }

        _cameraVerticalOffset = Mathf.Clamp(_cameraVerticalOffset, -1.00f, 1.00f);

        float blendFactor = 5.25f * _cameraVerticalOffset;
        blendFactor = Mathf.Clamp(blendFactor, -5.25f, 3.00f);
        blendFactor = Mathf.Round(blendFactor * 100.00f) / 100.00f;

        _cameraPositionComposer.TargetOffset = new Vector3(_cameraPositionComposer.TargetOffset.x, blendFactor, _cameraPositionComposer.TargetOffset.z);
    }
}
