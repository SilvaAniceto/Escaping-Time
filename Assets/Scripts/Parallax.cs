using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform _background1;
    [SerializeField] private Transform _background2;
    [SerializeField] private Transform _background3;
    [SerializeField] private Transform _background4;
    [SerializeField] private Transform _background5;
    [SerializeField, Range(-20, 20)] private float _background1Speed;
    [SerializeField, Range(-20, 20)] private float _background2Speed;
    [SerializeField, Range(-20, 20)] private float _background3Speed;
    [SerializeField, Range(-20, 20)] private float _background4Speed;
    [SerializeField, Range(-20, 20)] private float _background5Speed;

    private Vector3 _lastUpdatePosition = Vector3.zero;
    private Vector3 _deltaDistance;
    private float _currentSpeed;
    void Update()
    {
        if (IsCameraMoving())
        {
            _background1.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.05f, 0.05f) * _background1Speed * Time.deltaTime);
            _background2.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.05f, 0.05f) * _background2Speed * Time.deltaTime);
            _background3.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.05f, 0.05f) * _background3Speed * Time.deltaTime);
            _background4.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.05f, 0.05f) * _background4Speed * Time.deltaTime);
            _background5.Translate(Vector3.right * Mathf.Clamp(_deltaDistance.normalized.x, -0.05f, 0.05f) * _background5Speed * Time.deltaTime);
        }
    }
    
    private bool IsCameraMoving()
    {
        _deltaDistance = transform.position - _lastUpdatePosition;
        _currentSpeed = _deltaDistance.magnitude / Time.deltaTime;
        _lastUpdatePosition = transform.position;

        return Mathf.Round(_currentSpeed * 100) / 100 > 0.10f;
    }
}
