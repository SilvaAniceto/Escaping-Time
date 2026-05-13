using UnityEngine;
using UnityEngine.Events;

public class CharacterDamageManager : MonoBehaviour
{
    private CharacterContextManager _context;

    public bool IsInvincible { get; set; }
    public Vector3 SpawningPosition { get; set; }
    public float DamageHitDirection { get; set; }

    [HideInInspector] public UnityEvent OnResetState = new UnityEvent();

    public void Initialize(CharacterContextManager context)
    {
        _context = context;
    }

    public void ApplyDamage(float damageDirection)
    {
        if (IsInvincible)
        {
            return;
        }

        _context.PlayerInputManager.DisableInputAction();
        IsInvincible = true;
        DamageHitDirection = damageDirection;

        _context.CurrentState = new CharacterStateFactory(_context, _context.CurrentState.CharacterAnimationManager).DamagedState();
        _context.CurrentState.EnterState();
    }

    public void ResetCharacter()
    {
        _context.PlayerInputManager.DisableInputAction();
        _context.CurrentState = new CharacterStateFactory(_context, _context.CurrentState.CharacterAnimationManager).ResetState();
        _context.CurrentState.EnterState();
    }

    void OnDestroy()
    {
        OnResetState.RemoveAllListeners();
    }
}
