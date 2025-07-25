using Unity.Cinemachine;
using UnityEngine;

public class CameraBehaviourController : MonoBehaviour
{
    public static CameraBehaviourController Instance;

    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private CinemachinePositionComposer _cameraPositionComposer;
    [SerializeField] private CinemachineConfiner2D _confiner2D;

    private float _cameraVerticalOffset;

    public Transform CameraTarget
    {
        get
        {
            return _cinemachineCamera.Target.TrackingTarget;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

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

        float blendFactor = 16.00f * _cameraVerticalOffset;
        blendFactor = Mathf.Clamp(blendFactor, -4.00f, 3.00f);
        blendFactor = Mathf.Round(blendFactor * 100.00f) / 100.00f;

        _cameraPositionComposer.TargetOffset = new Vector3(_cameraPositionComposer.TargetOffset.x, blendFactor, _cameraPositionComposer.TargetOffset.z);
    }

    public void SetCinemachineTarget(Transform target)
    {
        _cinemachineCamera.Target.TrackingTarget = target;
    }

    public void SetCameraConfiner2D(Collider2D collider)
    {
        _confiner2D.BoundingShape2D = collider;
    }
}
