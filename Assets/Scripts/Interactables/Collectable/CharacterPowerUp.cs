using UnityEngine;

public class CharacterPowerUp : InteractableItem, ICharacterPowerUp
{
    [SerializeField] private EPowerUpType _powerUpType;
    [SerializeField] private EPowerUp _powerUp;

    [SerializeField, Range(3, 10)] private float _powerUpTemporaryTime = 3;

    [SerializeField] private bool _rechargable = false;

    public EPowerUpType PowerUpType { get => _powerUpType; set => _powerUpType = value; }
    public EPowerUp PowerUp { get => _powerUp; set => _powerUp = value; }

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);

        Activated = true;

        _rechargable = _powerUpType == EPowerUpType.Infinity ? false : _rechargable;
    }

    public override void ConfirmInteraction()
    {

    }

    public override void InteractablePauseState(bool value)
    {
        base.InteractablePauseState(value);
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        if (!Activated)
        {
            return;
        }

        Activated = false;

        switch (PowerUpType)
        {
            case EPowerUpType.Infinity:
                switch (PowerUp)
                {
                    case EPowerUp.AirJump:
                        characterContextManager.HasInfinityAirJump = true;
                        break;
                    case EPowerUp.Dash:
                        characterContextManager.HasInfinityDash = true;
                        break;
                    case EPowerUp.WallMove:
                        characterContextManager.HasInfinityWallMove = true;
                        break;
                }
                break;
            case EPowerUpType.Temporary:
                switch (PowerUp)
                {
                    case EPowerUp.AirJump:
                        if (!characterContextManager.HasInfinityAirJump)
                        {
                            characterContextManager.SetTemporaryAirJump(_powerUpTemporaryTime);
                        }
                        break;
                    case EPowerUp.Dash:
                        if (!characterContextManager.HasInfinityDash)
                        {
                            characterContextManager.SetTemporaryDash(_powerUpTemporaryTime);
                        }
                        break;
                    case EPowerUp.WallMove:
                        if (!characterContextManager.HasInfinityWallMove)
                        {
                            characterContextManager.SetTemporaryWallMove(_powerUpTemporaryTime);
                        }
                        break;
                }

                if (_rechargable)
                {
                    System.Action action = () =>
                    {
                        
                    };
                    characterContextManager.OnPowerUpInteractableRecharge.AddListener(this.RechargePowerUpInteractable);
                }

                break;
        }

        Animator.Play("PowerUp_Unlit");
        characterContextManager.GameContextManager.GameAudioManager.StopSFX();
        characterContextManager.GameContextManager.GameAudioManager.PlaySFX("PwrUp_Collect");
    }
    public void RechargePowerUpInteractable()
    {
        Animator.enabled = true;
        Animator.Play("PowerUp_Lit");
        Activated = true;
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
