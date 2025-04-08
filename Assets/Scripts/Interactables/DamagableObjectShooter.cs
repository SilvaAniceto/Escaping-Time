using System.Collections.Generic;
using UnityEngine;

public class DamagableObjectShooter : MonoBehaviour
{
    [SerializeField] private DamagableObject _damagableObject;
    [SerializeField, Range(1, 20)] private int _damagableObjectsCount = 1;
    [SerializeField] AnimationCurve _projectileCurve;
    [SerializeField, Range(0.1f, 5)] private float _shootInterval = 1.5f;

    private List<ShooterProjectile> _damagableObjects = new List<ShooterProjectile>();
    private float _shootTimer;
    private int _projectileIndex = 0;

    void Awake()
    {
        DamagableObject damagable;
        for (int i = 0; i < _damagableObjectsCount; i++)
        {
            damagable = Instantiate(_damagableObject);

            Physics2D.IgnoreCollision(damagable.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());

            ShooterProjectile projectile = damagable.gameObject.AddComponent<ShooterProjectile>();

            projectile.ProjectileCurve = _projectileCurve;

            damagable.transform.SetParent(transform);
            damagable.gameObject.SetActive(false);
            _damagableObjects.Add(projectile);
        }

        _shootTimer = _shootInterval;

        GameManagerContext.OnRunOrPauseStateChanged.AddListener(OnPauseState);
    }

    void Update()
    {
        _shootTimer -= Time.deltaTime;
        _shootTimer = Mathf.Clamp(_shootTimer, 0, 5);

        if (_shootTimer <= 0)
        {
            _shootTimer = _shootInterval;

            if (_projectileIndex + 1 >= _damagableObjects.Count)
            {
                _projectileIndex = 0;
            }
            else
            {
                _projectileIndex++;
            }

            if (!_damagableObjects[_projectileIndex].gameObject.activeInHierarchy)
            {
                _damagableObjects[_projectileIndex].gameObject.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        
    }

    private void OnPauseState(bool value)
    {
        foreach (ShooterProjectile obj in _damagableObjects)
        {
            obj.enabled = value;
        }
        this.enabled = value;
    }

    [RequireComponent(typeof(Rigidbody2D))]
    private class ShooterProjectile : MonoBehaviour
    {
        private float _speed = 0.00f;

        public float MoveSpeed
        {
            get
            {
                _speed += Time.deltaTime / 0.64f;
                _speed = Mathf.Clamp01(_speed);

                return ProjectileCurve.Evaluate(_speed);
            }
        }
        public AnimationCurve ProjectileCurve { get; set; }
        private Rigidbody2D Rigidbody2D { get; set; }
        
        private void Start()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void FixedUpdate()
        {
            Rigidbody2D.MovePosition(Rigidbody2D.position + (-(Vector2)transform.parent.right * Mathf.Lerp(3.50f, 8.00f, MoveSpeed)) * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ground")  || collision.CompareTag("Ceiling"))
            {
                gameObject.SetActive(false);
				transform.localPosition = Vector3.zero;
				transform.rotation = transform.parent.rotation;
			}
        }
    }
}
