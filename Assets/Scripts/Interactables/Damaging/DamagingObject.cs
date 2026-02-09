using UnityEngine;

public class DamagingObject : InteractableItem
{
    enum EDamageHitDirection
    {
        None,
        Left,
        Right,
        Both
    }

    [SerializeField] protected Transform _targetPosition;
    [SerializeField] private bool _ignoreDashState = false;
    [SerializeField] private EDamageHitDirection _damageHitDirection;
    [SerializeField, Range (1, 5)] private int _hitMagnitude = 1;

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);
    }

    private void OnEnable()
    {
        Collider.enabled = true;
    }

    private void OnDestroy()
    {

    }
    public override void SetCharacterReset(CharacterContextManager characterContextManager)
    {
        characterContextManager.SpawningPosition = _targetPosition.position;
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        if (characterContextManager.CurrentState == characterContextManager.CurrentState.CharacterStateFactory.DamagedState() || characterContextManager.CurrentState == characterContextManager.CurrentState.CharacterStateFactory.ResetState()) return;

        if (_ignoreDashState)
        {
            if (characterContextManager.CurrentState == characterContextManager.CurrentState.CharacterStateFactory.DashState()) return;
        }

        characterContextManager.GameAudioManager.StopCharacterSFX();
        characterContextManager.GameAudioManager.PlayCharacterSFX("Damage");

        if (_targetPosition != null)
        {
            SetCharacterReset(characterContextManager);
        }

        float currentDirection = 0;

        switch (_damageHitDirection)
        {
            case EDamageHitDirection.None:
                currentDirection = 0.00f;
                break;
            case EDamageHitDirection.Left:
                currentDirection = -1.00f * _hitMagnitude;
                break;
            case EDamageHitDirection.Right:
                currentDirection = 1.00f * _hitMagnitude;
                break;
            case EDamageHitDirection.Both:
                float direction = characterContextManager.transform.position.x > transform.position.x ? 1.00f : -1.00f;
                currentDirection = direction * _hitMagnitude;
                break;
        }

        characterContextManager.ApplyDamage(currentDirection);
    }
    public override void ConfirmInteraction()
    {

    }

    public override void InteractablePauseState(bool value)
    {
        base.InteractablePauseState(value);
    }
}
