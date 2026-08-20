using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField] private bool _isManager = false;
    [SerializeField] private List<ShooterBehavior> _shooters = new List<ShooterBehavior>();
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField, Range(1, 20)] private int _projectilesCount = 1;
    [SerializeField] private AnimationCurve _projectileCurve;
    [SerializeField, Range(3, 12)] private float _projectileMaxSpeed;
    [SerializeField, Range(0.08f, 5)] private float _shotInterval = 1.5f;
    [SerializeField] private bool _separatedShots = false;

    private List<ShooterProjectile> _projectiles = new List<ShooterProjectile>();
    private int _projectileIndex = 0;
    private Coroutine _shotCoroutine;
    private AudioSource _audioSource;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter, EInteractionType.Exit };
    public EInteractionType[] InteractionType { get => _interactionType; }

    private bool _shooting = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        SetProjectilesPool();

        if (!_isManager) return;

        if (_shooters.Count == 0)
        {
            _shooters.Add(this);
        }

        if (_separatedShots)
        {
            foreach (var shooter in _shooters)
            {
                GameEventsManager.OnPauseStateChanged.AddListener(shooter.OnPauseState);
            }
        }
        else
        {
            foreach (var shooter in _shooters)
            {
                GameEventsManager.OnPauseStateChanged.AddListener(shooter.OnPauseState);
            }
        }
    }

    private void OnDestroy()
    {
        GameEventsManager.OnPauseStateChanged.RemoveListener(OnPauseState);
    }

    private void OnPauseState(bool value)
    {
        foreach (ShooterProjectile proj in _projectiles)
        {
            proj.enabled = value;
        }
        this.enabled = value;
    }

    private void SetProjectilesPool()
    {
        for (int i = 0; i < _projectilesCount; i++)
        {
            GameObject projectileObj = Instantiate(_projectilePrefab);
            Physics2D.IgnoreCollision(projectileObj.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            ShooterProjectile projectile = projectileObj.AddComponent<ShooterProjectile>();
            projectile.Enable = false;

            projectile.Initialize(this, _projectileCurve, _projectileMaxSpeed);

            _projectiles.Add(projectile);
        }
    }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_isManager) return;

        switch (interactionType)
        {
            case EInteractionType.Enter:
                if (_isManager)
                {
                    foreach (var shooter in _shooters)
                    {
                        shooter.ShootProjectile();
                    }
                }
                else
                {
                    ShootProjectile();
                }
                break;
            case EInteractionType.Exit:
                if (_isManager)
                {
                    foreach (var shooter in _shooters)
                    {
                        shooter.StopShooting();
                    }
                }
                else
                {
                    StopShooting();
                }
                break;
        }
    }

    private void ShootProjectile()
    {
        _shooting = true;
        _shotCoroutine = StartCoroutine(ShootingRoutine());
    }

    private void StopShooting()
    {
        _shooting = false;
        if (_shotCoroutine != null)
        {
            StopCoroutine(_shotCoroutine);
            _shotCoroutine = null;
        }

        foreach (var shooter in _shooters)
        {
            shooter._projectiles[_projectileIndex].ResetProjectile();
        }
    }

    private IEnumerator ShootingRoutine()
    {
        while (_shooting)
        {
            if (_separatedShots)
            {
                yield return StartCoroutine(SeparatedShootingRoutine());
                yield break;
            }

            _projectiles[_projectileIndex].transform.localScale = Vector3.one * 1.6f;
            _projectiles[_projectileIndex].gameObject.SetActive(true);

            ServiceLocator.AudioManager.StopSFX(_audioSource);
            ServiceLocator.AudioManager.PlaySFX("Fire_Shot", _audioSource);

            yield return new WaitForSeconds(ServiceLocator.AudioManager.AudioClipLength("Fire_Shot") * 0.15f);

            _projectiles[_projectileIndex].transform.localScale = Vector3.one * 2.0f;
            _projectiles[_projectileIndex].Enable = true;

            _projectileIndex = (_projectileIndex + 1) % _projectiles.Count;

            yield return new WaitUntil(() => !_projectiles[_projectileIndex].gameObject.activeInHierarchy);

            yield return new WaitForSeconds(_shotInterval);
        }
    }

    private IEnumerator SeparatedShootingRoutine()
    {
        foreach (var shooter in _shooters)
        {
            shooter._projectiles[_projectileIndex].transform.localScale = Vector3.one * 1.6f;
            shooter._projectiles[_projectileIndex].gameObject.SetActive(true);

            ServiceLocator.AudioManager.StopSFX(shooter._audioSource);
            ServiceLocator.AudioManager.PlaySFX("Fire_Shot", shooter._audioSource);

            yield return new WaitForSeconds(ServiceLocator.AudioManager.AudioClipLength("Fire_Shot") * 0.15f);

            shooter._projectiles[_projectileIndex].transform.localScale = Vector3.one * 2.0f;
            shooter._projectiles[_projectileIndex].Enable = true;

            _projectileIndex = (_projectileIndex + 1) % _projectiles.Count;

            yield return new WaitUntil(() => !shooter._projectiles[_projectileIndex].gameObject.activeInHierarchy);

            yield return new WaitForSeconds(_shotInterval);
        }

        ShootProjectile();
    }

    private class ShooterProjectile : MonoBehaviour
    {
        private float _speedFactor = 0.00f;
        private float _currentMoveSpeed = 0.00f;
        private float[] _projectileCurve;
        private float _projectileMaxSpeed;
        private Rigidbody2D _rigidbody;
        
        public bool Enable { get; set; }

        public void Initialize(ShooterBehavior shooter, AnimationCurve projectileCurve, float projectileSpeed) 
        {
            _projectileCurve = CalculateLUT(projectileCurve);
            _projectileMaxSpeed = projectileSpeed;

            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;

            transform.SetParent(shooter.transform);
            ResetProjectile();
        }
        private void Update()
        {
            _speedFactor += Time.deltaTime / (1 / 60f);
            _speedFactor = Mathf.Clamp01(_speedFactor);

            _currentMoveSpeed = EvaluateLUT(_projectileCurve, _speedFactor);
        }

        private void FixedUpdate()
        {
            if (!Enable) return;

            _rigidbody.MovePosition(_rigidbody.position + (-(Vector2)transform.parent.right * Mathf.Lerp(0.00f, _projectileMaxSpeed, _currentMoveSpeed)) * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ground") || collision.CompareTag("Ceiling"))
            {
                StartCoroutine(ResetProjectileCoroutine());
            }
        }

        private IEnumerator ResetProjectileCoroutine()
        {
            yield return new WaitForEndOfFrame();
            ResetProjectile();
        }

        public void ResetProjectile()
        {
            _speedFactor = 0.00f;
            gameObject.SetActive(false);
            transform.localPosition = Vector3.zero;
            transform.rotation = transform.parent.rotation;
            Enable = false;
        }

        private float[] CalculateLUT(AnimationCurve curve)
        {
            float[] lut = new float[128];
            for (int i = 0; i < lut.Length; i++)
            {
                float t = (float)i / (lut.Length - 1);
                lut[i] = curve.Evaluate(t);
            }
            return lut;
        }
        private float EvaluateLUT(float[] lut, float time)
        {
            time = Mathf.Clamp01(time);
            float index = time * (lut.Length - 1);
            int prev = (int)index;
            int next = Mathf.Min(prev + 1, lut.Length - 1);
            float frac = index - prev;
            return Mathf.Lerp(lut[prev], lut[next], frac);
        }
    }
}