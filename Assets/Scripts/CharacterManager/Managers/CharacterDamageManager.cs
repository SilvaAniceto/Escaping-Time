using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterDamageManager : MonoBehaviour
{
    private CharacterContextManager _context;

    private bool _isInvincible;
    private Coroutine _invincibilityCoroutine;

    public bool IsInvincible => _isInvincible;
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
        _isInvincible = true;
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

    public void SetInvincibleForSeconds(float duration)
    {
        _isInvincible = true;
        if (_invincibilityCoroutine != null)
        {
            StopCoroutine(_invincibilityCoroutine);
        }
        _invincibilityCoroutine = StartCoroutine(ResetInvincibilityAfter(duration));
    }

    private IEnumerator ResetInvincibilityAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isInvincible = false;
    }
    public void ResetInvincibility()
    {
        _isInvincible = false;
        if (_invincibilityCoroutine != null)
        {
            StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = null;
        }
    }

    void OnDestroy()
    {
        OnResetState.RemoveAllListeners();
    }
}
