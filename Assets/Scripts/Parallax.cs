using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform _background1;
    [SerializeField] private Transform _background2;
    [SerializeField] private Transform _background3;
    [SerializeField] private Transform _background4;
    [SerializeField] private Transform _background5;

    private Vector3 _lastUpdatePosition = Vector3.zero;
    private Vector3 _deltaDistance;
    private float _currentSpeed;
    void Update()
    {
        if (IsCameraMoving())
        {
            _background1.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.5f, 0.5f) * -0.001f * Time.deltaTime);
            _background2.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.5f, 0.5f) * -2.6f * Time.deltaTime);
            _background3.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.5f, 0.5f) * -2.8f * Time.deltaTime);
            _background4.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.5f, 0.5f) * -3.14f * Time.deltaTime);
            _background5.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.5f, 0.5f) * -3.14f * Time.deltaTime);
        }
    }
    
    private bool IsCameraMoving()
    {
        _deltaDistance = transform.position - _lastUpdatePosition;
        _currentSpeed = _deltaDistance.magnitude / Time.deltaTime;
        _lastUpdatePosition = transform.position;

        return Mathf.Round(_currentSpeed * 100) / 100 > 0.10f;
    }
    private void OnGUI()
    {
        GUILayout.Label(Mathf.Clamp01(_deltaDistance.magnitude * _deltaDistance.normalized.x).ToString());
    }
}
