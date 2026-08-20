using System.Linq;
using UnityEngine;

public class PowerUpBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField] private EPowerUpType _powerUpType;
    [SerializeField] private EPowerUp _powerUp;

    [SerializeField, Range(3, 10)] private float _powerUpTemporaryTime = 3;
    [SerializeField] private bool _rechargable = false;

    private bool _activated = true;
    private Animator _animator;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rechargable = _powerUpType == EPowerUpType.Infinity ? false : _rechargable;
    }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        if (!_activated)
        {
            return;
        }

        _activated = false;

        switch (_powerUpType)
        {
            case EPowerUpType.Infinity:
                switch (_powerUp)
                {
                    case EPowerUp.AirJump:
                        context.PowerUpManager.HasInfinityAirJump = true;
                        break;
                    case EPowerUp.Dash:
                        context.PowerUpManager.HasInfinityDash = true;
                        break;
                    case EPowerUp.WallMove:
                        context.PowerUpManager.HasInfinityWallMove = true;
                        break;
                }
                break;
            case EPowerUpType.Temporary:
                switch (_powerUp)
                {
                    case EPowerUp.AirJump:
                        if (!context.PowerUpManager.HasInfinityAirJump)
                        {
                            context.PowerUpManager.SetTemporaryAirJump(_powerUpTemporaryTime);
                        }
                        break;
                    case EPowerUp.Dash:
                        if (!context.PowerUpManager.HasInfinityDash)
                        {
                            context.PowerUpManager.SetTemporaryDash(_powerUpTemporaryTime);
                        }
                        break;
                    case EPowerUp.WallMove:
                        if (!context.PowerUpManager.HasInfinityWallMove)
                        {
                            context.PowerUpManager.SetTemporaryWallMove(_powerUpTemporaryTime);
                        }
                        break;
                }

                if (_rechargable)
                {
                    context.PowerUpManager.OnPowerUpInteractableRecharge.AddListener(RechargePowerUpInteractable);
                }
                break;
        }

        if (_animator != null)
        {
            _animator.Play("PowerUp_Unlit");
        }

        GameContextManager.Instance.AudioManager.StopSFX();
        GameContextManager.Instance.AudioManager.PlaySFX("PwrUp_Collect");
    }

    public void RechargePowerUpInteractable()
    {
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.Play("PowerUp_Lit");
        }

        _activated = true;
    }

    public void DestroyPowerUpInteractable()
    {
        if (_rechargable)
        {
            return;
        }

        gameObject.SetActive(false);
    }
}