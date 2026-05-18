using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterPowerUpManager : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private bool _hasInfinityDash;
    [Header("Air Jump")]
    [SerializeField] private bool _hasInfinityAirJump;
    [Header("Wall Move")]
    [SerializeField] private bool _hasInfinityWallMove;

    private bool _hasTemporaryDash;
    private bool _hasTemporaryAirJump;
    private bool _hasTemporaryWallMove;

    private bool _dashOnCoolDown;
    private bool _dashIsWaitingGroundedState;

    private bool _airJumpIsAllowed;

    [HideInInspector] public UnityEvent OnPowerUpInteractableRecharge = new UnityEvent();
    [HideInInspector] public UnityEvent<string> OnDashPowerStateChange = new UnityEvent<string>();
    [HideInInspector] public UnityEvent<string> OnAirJumpPowerStateChange = new UnityEvent<string>();
    [HideInInspector] public UnityEvent<string> OnWallMovePowerStateChange = new UnityEvent<string>();

    public bool HasInfinityDash
    {
        get => _hasInfinityDash;
        set
        {
            if (_hasInfinityDash == value)
            {
                return;
            }

            _hasInfinityDash = value;

            if (_hasInfinityDash)
            {
                OnDashPowerStateChange.Invoke("PwrUp_Infinity");
            }
        }
    }
    public bool HasTemporaryDash
    {
        get => _hasTemporaryDash;
        set
        {
            if (value == _hasTemporaryDash || HasInfinityDash)
            {
                return;
            }

            _hasTemporaryDash = value;

            OnDashPowerStateChange.Invoke(_hasTemporaryDash ? "PwrUp_UI_Lit" : "PwrUp_UI_Unlit");
        }
    }
    public bool HasDash => HasTemporaryDash || HasInfinityDash;
    public bool DashIsAllowed => HasDash && !_dashOnCoolDown && !_dashIsWaitingGroundedState;

    public bool HasInfinityAirJump
    {
        get => _hasInfinityAirJump;
        set
        {
            if (_hasInfinityAirJump == value)
            {
                return;
            }

            _hasInfinityAirJump = value;

            if (_hasInfinityAirJump)
            {
                OnAirJumpPowerStateChange.Invoke("PwrUp_Infinity");
            }
        }
    }
    public bool HasTemporaryAirJump
    {
        get => _hasTemporaryAirJump;
        set
        {
            if (value == _hasTemporaryAirJump || HasInfinityAirJump)
            {
                return;
            }

            _hasTemporaryAirJump = value;

            OnAirJumpPowerStateChange.Invoke(_hasTemporaryAirJump ? "PwrUp_UI_Lit" : "PwrUp_UI_Unlit");
        }
    }
    public bool HasAirJump => HasTemporaryAirJump || HasInfinityAirJump;
    public bool AirJumpIsAllowed => HasAirJump && _airJumpIsAllowed;

    public bool HasInfinityWallMove
    {
        get => _hasInfinityWallMove;
        set
        {
            if (_hasInfinityWallMove == value)
            {
                return;
            }

            _hasInfinityWallMove = value;

            if (_hasInfinityWallMove)
            {
                OnWallMovePowerStateChange.Invoke("PwrUp_Infinity");
            }
        }
    }
    public bool HasTemporaryWallMove
    {
        get => _hasTemporaryWallMove;
        set
        {
            if (value == _hasTemporaryWallMove || HasInfinityWallMove)
            {
                return;
            }

            _hasTemporaryWallMove = value;

            OnWallMovePowerStateChange.Invoke(_hasTemporaryWallMove ? "PwrUp_UI_Lit" : "PwrUp_UI_Unlit");
        }
    }
    public bool HasWallMove => HasInfinityWallMove || HasTemporaryWallMove;

    public void SetDashCooldown()
    {
        _dashOnCoolDown = true;
    }
    public void SetDashWaitingForGround()
    {
        _dashIsWaitingGroundedState = true;
    }
    public void ResetDashOnLand()
    {
        _dashIsWaitingGroundedState = false;
    }
    public void ResetDashCoolDown()
    {
        _dashOnCoolDown = false;
    }
    public void SetTemporaryDash(float coolDown = 0)
    {
        HasTemporaryDash = true;
        if (coolDown > 0)
        {
            StartCoroutine(ResetTemporaryDashAfter(coolDown));
        }
    }
    private IEnumerator ResetTemporaryDashAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        HasTemporaryDash = false;
    }
    public void RegisterDashCallback()
    {
        OnDashPowerStateChange.AddListener(GameContextManager.Instance.UIManager.SetDashPowerUpUI);
    }

    public void EnableAirJump()
    {
        _airJumpIsAllowed = true;
    }
    public void DisableAirJump()
    {
        _airJumpIsAllowed = false;
    }
    public void SetTemporaryAirJump(float coolDown = 0)
    {
        HasTemporaryAirJump = true;
        if (coolDown > 0)
        {
            StartCoroutine(ResetTemporaryAirJumpAfter(coolDown));
        }
    }
    private IEnumerator ResetTemporaryAirJumpAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        HasTemporaryAirJump = false;
    }
    public void RegisterAirJumpCallback()
    {
        OnAirJumpPowerStateChange.AddListener(GameContextManager.Instance.UIManager.SetAirJumpPowerUpUI);
    }

    public void SetTemporaryWallMove(float coolDown = 0)
    {
        HasTemporaryWallMove = true;
        if (coolDown > 0)
        {
            StartCoroutine(ResetTemporaryWallMoveAfter(coolDown));
        }
    }
    private IEnumerator ResetTemporaryWallMoveAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        HasTemporaryWallMove = false;
    }
    public void RegisterWallMoveCallback()
    {
        OnWallMovePowerStateChange.AddListener(GameContextManager.Instance.UIManager.SetWallMovePowerUpUI);
    }

    void OnDestroy()
    {
        OnDashPowerStateChange.RemoveAllListeners();
        OnAirJumpPowerStateChange.RemoveAllListeners();
        OnWallMovePowerStateChange.RemoveAllListeners();
        OnPowerUpInteractableRecharge.RemoveAllListeners();
    }

    public void DispatchPowerUpInteractableRecharge()
    {
        OnPowerUpInteractableRecharge?.Invoke();
        OnPowerUpInteractableRecharge.RemoveAllListeners();
    }
}
