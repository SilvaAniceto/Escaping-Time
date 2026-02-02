using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagingObjectShooter : InteractableItem
{
    [SerializeField] private bool _isManager = false;
    [SerializeField] private List<DamagingObjectShooter> _shooters = new List<DamagingObjectShooter>();
    [SerializeField] private DamagingObject _damagingObject;
    [SerializeField, Range(1, 20)] private int _damagingObjectsCount = 1;
    [SerializeField] AnimationCurve _projectileCurve;
    [SerializeField, Range(3, 12)] private float _projectileMaxSpeed;
    [SerializeField, Range(0.08f, 5)] private float _shotInterval = 1.5f;
    [SerializeField] private bool _separatedShots = false;

    protected List<ShooterProjectile> DamagingObjects = new List<ShooterProjectile>();
    private int _projectileIndex = 0;

    private UnityEvent<GameContextManager> OnShootProjectile = new UnityEvent<GameContextManager>();
    private UnityEvent OnStopShootingProjectile = new UnityEvent();

    private Coroutine ShotCoroutine;

    #region DEFAULT METHODS
    public override void Awake()
    {
        base.Awake();

        SetDamagingObjectsPool();

        if (!_isManager) return;

        Interactions.Add(EInteractionType.Enter);
        Interactions.Add(EInteractionType.Exit);

        if (_shooters.Count == 0)
        {
            _shooters.Add(this);
        }

        if (_separatedShots)
        {
            OnShootProjectile.AddListener(SeparatedProjectile);
            OnStopShootingProjectile.AddListener(StopShooting);

            foreach (var shooter in _shooters)
            {
                GameContextManager.OnRunOrPauseStateChanged.AddListener(shooter.OnPauseState);
            }
        }
        else
        {
            foreach (var shooter in _shooters)
            {
                OnShootProjectile.AddListener(shooter.ShootProjectile);
                OnStopShootingProjectile.AddListener(shooter.StopShooting);

                GameContextManager.OnRunOrPauseStateChanged.AddListener(shooter.OnPauseState);
            }
        }
    }
    private void OnDestroy()
    {
        
    }
    #endregion

    #region INTERACTABLE ITEM METHODS
    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        if (!_isManager) return;

        switch (interactionType)
        {
            case EInteractionType.Enter:
                OnShootProjectile?.Invoke(characterContextManager.GameContextManager);
                break;
            case EInteractionType.Stay:
                break;
            case EInteractionType.Exit:
                OnStopShootingProjectile?.Invoke();
                break;
        }
    }
    #endregion

    #region SHOT METHODS
    private void OnPauseState(bool value)
    {
        foreach (ShooterProjectile obj in DamagingObjects)
        {
            obj.enabled = value;
        }
        this.enabled = value;
    }
    protected void SetDamagingObjectsPool()
    {
        DamagingObject damagable;

        for (int i = 0; i < _damagingObjectsCount; i++)
        {
            damagable = Instantiate(_damagingObject);

            Physics2D.IgnoreCollision(damagable.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());

            ShooterProjectile projectile = damagable.gameObject.AddComponent<ShooterProjectile>();

            projectile.Enable = false;

            projectile.ProjectileCurve = _projectileCurve;
            projectile.ProjectileMaxSpeed = _projectileMaxSpeed;
            projectile.Shooter = this;

            projectile.transform.SetParent(transform);
            projectile.SetProjectile();

            DamagingObjects.Add(projectile);
        }
    }
    public void ShootProjectile(GameContextManager gameContextManager)
    {
        ShotCoroutine = StartCoroutine(ShootingRoutine(gameContextManager));
    }
    public void StopShooting()
    {
        StopCoroutine(ShotCoroutine);

        foreach (var shooter in _shooters)
        {
            shooter.DamagingObjects[_projectileIndex].SetProjectile();
        }
    }
    IEnumerator ShootingRoutine(GameContextManager gameContextManager)
    {
        DamagingObjects[_projectileIndex].transform.localScale = Vector3.one * 1.6f;
        DamagingObjects[_projectileIndex].gameObject.SetActive(true);

        gameContextManager.GameAudioManager.StopSFX(AudioSource);
        gameContextManager.GameAudioManager.PlaySFX("Fire_Shot", AudioSource);

        yield return new WaitForSeconds(gameContextManager.GameAudioManager.AudioClipLength("Fire_Shot") * 0.15f);

        DamagingObjects[_projectileIndex].transform.localScale = Vector3.one * 2.0f;
        DamagingObjects[_projectileIndex].Enable = true;

        if (_projectileIndex + 1 >= DamagingObjects.Count)
        {
            _projectileIndex = 0;
        }
        else
        {
            _projectileIndex++;
        }

        yield return new WaitUntil(() => !DamagingObjects[_projectileIndex].gameObject.activeInHierarchy);

        yield return new WaitForSeconds(_shotInterval);

        OnShootProjectile?.Invoke(gameContextManager);
    }
    public void SeparatedProjectile(GameContextManager gameContextManager)
    {
        ShotCoroutine = StartCoroutine(SeparatedShootingRoutine(gameContextManager));
    }
    IEnumerator SeparatedShootingRoutine(GameContextManager gameContextManager)
    {
        foreach (var shooter in _shooters)
        {
            shooter.DamagingObjects[_projectileIndex].transform.localScale = Vector3.one * 1.6f;
            shooter.DamagingObjects[_projectileIndex].gameObject.SetActive(true);

            gameContextManager.GameAudioManager.StopSFX(shooter.AudioSource);
            gameContextManager.GameAudioManager.PlaySFX("Fire_Shot", shooter.AudioSource);

            yield return new WaitForSeconds(gameContextManager.GameAudioManager.AudioClipLength("Fire_Shot") * 0.15f);

            shooter.DamagingObjects[_projectileIndex].transform.localScale = Vector3.one * 2.0f;
            shooter.DamagingObjects[_projectileIndex].Enable = true;

            if (_projectileIndex + 1 >= DamagingObjects.Count)
            {
                _projectileIndex = 0;
            }
            else
            {
                _projectileIndex++;
            }

            yield return new WaitUntil(() => !shooter.DamagingObjects[_projectileIndex].gameObject.activeInHierarchy);

            yield return new WaitForSeconds(_shotInterval);
        }
        OnShootProjectile?.Invoke(gameContextManager);
    }
    #endregion

    #region PROJECTILE METHODS
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShooterProjectile : MonoBehaviour
    {
        private float _speed = 0.00f;

        public float MoveSpeed
        {
            get
            {
                _speed += Time.deltaTime / (1/60);
                _speed = Mathf.Clamp01(_speed);

                return ProjectileCurve.Evaluate(_speed);
            }
        }
        public bool Enable { get; set; }
        public DamagingObjectShooter Shooter { get; set; }
        public AnimationCurve ProjectileCurve { get; set; }
        public float ProjectileMaxSpeed { get; set; }
        private Rigidbody2D Rigidbody2D { get; set; }

        private void Start()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
            Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void FixedUpdate()
        {
            if (!Enable) return;

            Rigidbody2D.MovePosition(Rigidbody2D.position + (-(Vector2)transform.parent.right * Mathf.Lerp(0.00f, ProjectileMaxSpeed, MoveSpeed)) * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ground")  || collision.CompareTag("Ceiling"))
            {
                StartCoroutine(ResetProjectile());
			}
        }

        IEnumerator ResetProjectile()
        {
            yield return new WaitForEndOfFrame();
            _speed = 0.00f;
            gameObject.SetActive(false);
            transform.localPosition = Vector3.zero;
            transform.rotation = transform.parent.rotation;
            Enable = false;
        }

        public void SetProjectile()
        {
            _speed = 0.00f;
            gameObject.SetActive(false);
            transform.localPosition = Vector3.zero;
            transform.rotation = transform.parent.rotation;
            Enable = false;
        }
    }
    #endregion
}
