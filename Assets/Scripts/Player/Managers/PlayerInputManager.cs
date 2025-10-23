using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager PlayerInputInstance;

    public PlayerInputActions PlayerInputActions { get; private set; }

    public bool Cancel { get => PlayerInputActions.PlayerActionMap.Cancel.triggered; }
    public float MoveInput
    {
        get
        {
            Vector2 move = new Vector2(PlayerInputActions.PlayerActionMap.Move.ReadValue<float>(), 0.00f);
            move = move.normalized;
            return move.x;
        }
    }
    public bool HoldJumpInput { get => PlayerInputActions.PlayerActionMap.Jump.IsPressed(); }
    public bool StartJumpInput { get => PlayerInputActions.PlayerActionMap.Jump.WasPressedThisFrame(); }
    public bool InteractionInput { get => PlayerInputActions.PlayerActionMap.Interact.WasPressedThisFrame(); }
    public bool DashInput { get => PlayerInputActions.PlayerActionMap.Dash.WasPressedThisFrame(); }
    public float CameraTiltInput { get => PlayerInputActions.PlayerActionMap.CameraTilt.ReadValue<float>(); }
    public bool WallMoveInput { get => PlayerInputActions.PlayerActionMap.WallMove.IsPressed(); }

    void Awake()
    {
        if (PlayerInputInstance == null)
        {
            PlayerInputInstance = this;
        }
        else
        {
            Destroy(PlayerInputInstance);
        }

        if (PlayerInputActions == null)
        {
            PlayerInputActions = new PlayerInputActions();
        }
    }

    private void OnEnable()
    {
        PlayerInputActions.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions.Disable();
    }

    private void OnDestroy()
    {
        PlayerInputActions.Disable();
    }
}