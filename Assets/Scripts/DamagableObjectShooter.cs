using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DamagableObjectShooter : MonoBehaviour
{
    [SerializeField] private DamagableObject _damagableObject;
    [SerializeField, Range(1, 20)] private int _damagableObjectsCount = 1;
    [SerializeField, Range(3.14f, 9.42f)] private float _damagableSpeed = 1;
    [SerializeField] private LayerMask _damagableLayerCollision;
    [SerializeField, Range(0.1f, 5)] private float _shootInterval = 1.5f;

    private List<DamagableObject> _damagableObjects = new List<DamagableObject>();
    private float _shootTimer;
    private int _projectileIndex = 0;

    void Awake()
    {
        DamagableObject damagable;
        for (int i = 0; i < _damagableObjectsCount; i++)
        {
            damagable = Instantiate(_damagableObject);

            Physics2D.IgnoreCollision(damagable.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());

            damagable.MovableObject = true;
            ShooterProjectile projectile = damagable.gameObject.AddComponent<ShooterProjectile>();

            projectile.MoveSpeed = _damagableSpeed;
            projectile.DamagableLayerCollision = _damagableLayerCollision;

            damagable.transform.SetParent(transform);
            damagable.gameObject.SetActive(false);
            _damagableObjects.Add(damagable);
        }

        _shootTimer = _shootInterval;
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

    private class ShooterProjectile : MonoBehaviour
    {
        public float MoveSpeed { get; set; }
        public LayerMask DamagableLayerCollision { get; set; }

        private void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            transform.rotation = transform.parent.rotation;
        }


        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position - transform.parent.right, MoveSpeed * Time.deltaTime);

            if (Physics2D.OverlapCircle(transform.position, gameObject.GetComponent<CircleCollider2D>().radius, DamagableLayerCollision))
            {
                gameObject.SetActive(false);
            }
        }
    }
}
