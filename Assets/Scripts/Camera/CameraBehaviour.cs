using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _cameraTarget;
    [SerializeField, Range(0.00f, 5.00f)] private float _horizontalOffset;
    [SerializeField, Range(0.00f, 5.00f)] private float _verticalOffset;
    [SerializeField, Range(5.00f, 15.00f)] private float _cameraDistance;

    private PlayerInputManager _playerInputManager;
    
    private Vector3 TargetOffset
    {
        get
        {
            _cameraTarget.localPosition = new Vector3(_horizontalOffset, _verticalOffset, 0);
            return _cameraTarget.position + Vector3.back * _cameraDistance;
        }
    }
    void LateUpdate()
    {
        float cameraXPosition = Mathf.Lerp(transform.position.x, TargetOffset.x, 1-Mathf.Pow(0.5f, Time.deltaTime * 10));

        transform.position = new Vector3(cameraXPosition, TargetOffset.y, TargetOffset.z);
    }
}
