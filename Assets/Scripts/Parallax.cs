using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
        if (_background1.position.y > 10)
        {
            _background1.position = new Vector3(_background1.position.x, 0, _background1.position.z);
            _background2.position = new Vector3(_background2.position.x, 0, _background2.position.z);
            _background3.position = new Vector3(_background3.position.x, 0, _background3.position.z);
            _background4.position = new Vector3(_background4.position.x, 0, _background4.position.z);
        }
        if (_background5.position.y > 2)
        {
            _background5.position = new Vector3(_background5.position.x, 0, _background5.position.z);
        }

        _background1.Translate(Vector3.up * _background1Speed * Time.deltaTime);
        _background2.Translate(Vector3.up * _background2Speed * Time.deltaTime);
        _background3.Translate(Vector3.up * _background3Speed * Time.deltaTime);
        _background4.Translate(Vector3.up * _background4Speed * Time.deltaTime);
        _background5.Translate(Vector3.up * _background5Speed * Time.deltaTime);
    }
    
    private bool IsCameraMoving()
    {
        _deltaDistance = transform.position - _lastUpdatePosition;
        _currentSpeed = _deltaDistance.magnitude / Time.deltaTime;
        _lastUpdatePosition = transform.position;

        return Mathf.Round(_currentSpeed * 100) / 100 > 0.10f;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Fase_0");
        SceneManager.UnloadSceneAsync("Menu");
    }
}
